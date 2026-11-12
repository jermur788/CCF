import os
import runpy
import subprocess
from pathlib import Path

directory = Path(__file__).resolve().parent
original_popen = subprocess.Popen


def followup_popen(arguments, *positional, **keywords):
    if isinstance(arguments, list):
        driver = 'followup_native_fast_427.py' if os.environ.get('CCF_427_FAST_DRIVER') == '1' else 'followup_native_427.py'
        arguments = [str(directory / driver) if str(argument) == str(directory / 'd073_native_input.py') else argument for argument in arguments]
        control = os.environ.get('CCF_427_FOCUS_CONTROL')
        # The first-run focus loop re-activates, moves and resizes the Editor window every ~2 s. With a control file, the loop only
        # runs while that file exists, so a test can switch the loop on and off and compare click loss with and without it.
        if control and len(arguments) == 3 and arguments[0] == 'bash' and 'windowactivate' in arguments[2]:
            arguments[2] = arguments[2].replace('do w=$(', 'do [ -e ' + control + ' ] || { sleep 0.2; continue; }; w=$(', 1)
    return original_popen(arguments, *positional, **keywords)


subprocess.Popen = followup_popen
runpy.run_path(str(directory / 'run_unity_427.py'), run_name='__main__')
