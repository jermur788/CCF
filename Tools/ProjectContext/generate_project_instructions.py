#!/usr/bin/env python3
"""Generate committed CCF instruction composites and a stamped mirror snapshot.

Based on the accepted Revision 5 Drive generator. Clean mode reads all sources
at one full context SHA; live coordination is deliberately outside that set.
Dirty preview mode never labels working-tree content as a committed snapshot.
Python 3.10+, standard library only.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import re
import subprocess
from pathlib import Path

AUTH_START = "<!-- PROJECT_AUTHORITY_BLOCK_START -->"
AUTH_END = "<!-- PROJECT_AUTHORITY_BLOCK_END -->"
CONTEXT_PATHS = [
    "Docs/Project/game-brief.md",
    "Docs/Project/decision-log.md",
    "Docs/Project/current-milestone.md",
    "Docs/Project/unity-project-overview.md",
    "Docs/Project/AgentWorkflow.md",
    "Docs/Project/research-index.md",
    "Docs/Project/instructions/shared-project-instructions.md",
    "Docs/Project/instructions/chatgpt-role.md",
    "Docs/Project/instructions/claude-role.md",
    "AGENTS.md",
    "CLAUDE.md",
    "Tools/ProjectContext/generate_project_instructions.py",
]
WORKFLOW = "Docs/Project/AgentWorkflow.md"
SHARED = "Docs/Project/instructions/shared-project-instructions.md"
CHATGPT_ROLE = "Docs/Project/instructions/chatgpt-role.md"
CLAUDE_ROLE = "Docs/Project/instructions/claude-role.md"


def git(repo: Path, *args: str) -> str:
    return git_raw(repo, *args).strip()


def git_raw(repo: Path, *args: str) -> str:
    return subprocess.check_output(["git", "-C", str(repo), *args], encoding="utf-8")


def require_sources_exist(repo: Path) -> None:
    missing = [p for p in CONTEXT_PATHS if not (repo / p).is_file()]
    if missing:
        raise RuntimeError("Missing canonical context source(s):\n- " + "\n- ".join(missing))


def dirty_context_paths(repo: Path) -> list[str]:
    return git(repo, "status", "--porcelain", "--untracked-files=all", "--", *CONTEXT_PATHS).splitlines()


def context_commit(repo: Path) -> str:
    sha = git(repo, "log", "-1", "--format=%H", "--", *CONTEXT_PATHS)
    if not sha:
        raise RuntimeError("No committed context snapshot exists yet.")
    if not re.fullmatch(r"[0-9a-f]{40}|[0-9a-f]{64}", sha):
        raise RuntimeError("Context snapshot must use a full Git SHA.")
    return sha


def committed_text(repo: Path, sha: str, path: str) -> str:
    try:
        return git_raw(repo, "show", f"{sha}:{path}")
    except subprocess.CalledProcessError as error:
        raise RuntimeError(f"Missing committed context source {path} at {sha}.") from error


def working_text(repo: Path, path: str) -> str:
    return (repo / path).read_text(encoding="utf-8")


def authority_block(workflow: str) -> str:
    if workflow.count(AUTH_START) != 1 or workflow.count(AUTH_END) != 1:
        raise RuntimeError("AgentWorkflow must contain exactly one authority marker pair.")
    start, end = workflow.index(AUTH_START) + len(AUTH_START), workflow.index(AUTH_END)
    if end <= start:
        raise RuntimeError("AgentWorkflow authority markers are malformed.")
    block = workflow[start:end].strip()
    if not block:
        raise RuntimeError("AgentWorkflow authority block is empty.")
    return block


def validate_shared_core(shared: str) -> None:
    if AUTH_START in shared or AUTH_END in shared:
        raise RuntimeError("Shared core must not contain authority markers.")
    duplicates = [heading for heading in ("### Decision authority", "### Implementation authority") if heading in shared]
    if duplicates:
        raise RuntimeError("Shared core duplicates authority text: " + ", ".join(duplicates))


def sha256_text(text: str) -> str:
    return hashlib.sha256(text.encode("utf-8")).hexdigest()


def generated_header(name: str, stamp: str, sources: list[str]) -> str:
    return (f"# {name}\n\nGENERATED FILE — DO NOT EDIT DIRECTLY\n\nContext: {stamp}\n\nSources:\n"
            + "\n".join(f"- {source}" for source in sources) + "\n")


def build_composite(name: str, stamp: str, role_path: str, role: str, workflow: str, shared: str) -> str:
    return "\n\n".join([
        generated_header(name, stamp, [role_path, WORKFLOW, SHARED]).strip(),
        role.strip(),
        "## Derived authority summary\n\n" + authority_block(workflow),
        shared.strip(),
    ]) + "\n"


def stamped_mirror(path: str, stamp: str, snapshot_time: str, source: str) -> str:
    return (f"MIRROR — DO NOT EDIT\n\nSource: {path}\nContext: {stamp}\n"
            f"Snapshot timestamp: {snapshot_time}\n\n" + source)


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--repo", type=Path, default=Path(__file__).resolve().parents[2])
    parser.add_argument("--out", type=Path, default=None)
    parser.add_argument("--preview-dirty", action="store_true",
                        help="Generate explicitly DIRTY PREVIEW working-tree content instead of a committed snapshot.")
    args = parser.parse_args()
    repo = args.repo.resolve()
    out = (args.out or (repo / "Build" / "ProjectContext")).resolve()
    try:
        require_sources_exist(repo)
        dirty = dirty_context_paths(repo)
        if dirty and not args.preview_dirty:
            raise RuntimeError("Refusing to generate a committed context from dirty/untracked context sources:\n"
                               + "\n".join(dirty)
                               + "\nCommit/revert them, or use --preview-dirty for a clearly labelled preview.")
        if args.preview_dirty:
            stamp = "DIRTY PREVIEW — NOT A CONTEXT COMMIT"
            snapshot_time = "DIRTY PREVIEW"
            source_text = {p: working_text(repo, p) for p in CONTEXT_PATHS}
        else:
            stamp = context_commit(repo)
            snapshot_time = git(repo, "show", "-s", "--format=%cI", stamp)
            source_text = {p: committed_text(repo, stamp, p) for p in CONTEXT_PATHS}
        validate_shared_core(source_text[SHARED])
        authority_block(source_text[WORKFLOW])
        generated = {
            "chatgpt-project-instructions.md": build_composite("ChatGPT Game Dev Project Instructions", stamp,
                CHATGPT_ROLE, source_text[CHATGPT_ROLE], source_text[WORKFLOW], source_text[SHARED]),
            "claude-project-instructions.md": build_composite("Claude Project Instructions", stamp,
                CLAUDE_ROLE, source_text[CLAUDE_ROLE], source_text[WORKFLOW], source_text[SHARED]),
        }
        mirrors = {f"mirrors/{p}": stamped_mirror(p, stamp, snapshot_time, source) for p, source in source_text.items()}
        manifest = {
            "context": stamp, "preview": bool(args.preview_dirty), "snapshot_timestamp": snapshot_time,
            "source_files": {p: sha256_text(t) for p, t in sorted(source_text.items())},
            "generated_files": {p: sha256_text(t) for p, t in sorted(generated.items())},
            "mirror_files": {p: sha256_text(t) for p, t in sorted(mirrors.items())},
        }
        # Validate everything before creating output. LF is explicit on every OS.
        for path, content in {**generated, **mirrors, "context-manifest.json": json.dumps(manifest, indent=2, sort_keys=True) + "\n"}.items():
            destination = out / path
            destination.parent.mkdir(parents=True, exist_ok=True)
            destination.write_text(content, encoding="utf-8", newline="\n")
    except (RuntimeError, OSError, subprocess.CalledProcessError) as error:
        parser.error(str(error))
    print(f"Context: {stamp}")
    for name in sorted(generated):
        print(out / name)
    print(out / "context-manifest.json")
    print(out / "mirrors")


if __name__ == "__main__":
    main()
