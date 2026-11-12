import runpy
import subprocess
from pathlib import Path

directory = Path(__file__).resolve().parent
original_popen = subprocess.Popen


def followup_popen(arguments, *positional, **keywords):
    if isinstance(arguments, list):
        arguments = [str(directory / 'followup_native_427.py') if str(argument) == str(directory / 'd073_native_input.py') else argument for argument in arguments]
    return original_popen(arguments, *positional, **keywords)


subprocess.Popen = followup_popen
runpy.run_path(str(directory / 'run_unity_427.py'), run_name='__main__')
