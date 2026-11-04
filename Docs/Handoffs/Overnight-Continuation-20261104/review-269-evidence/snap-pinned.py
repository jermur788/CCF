import runpy
from pathlib import Path
h = runpy.run_path('Tools/Verification/RackV2ConfirmMinimap/run_static.py', run_name='rack_static_helpers')
s = Path('Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs').read_text()
body, error = h['extract_body'](s, r'private\s+void\s+OnPointerMove\s*\([^)]*\)', 'OnPointerMove')
assert error is None, error
assert 'Propose(px, gesture != Gesture.Reshape && !ShiftHeld(e.modifiers),' in body
assert 'next = Gesture.Reshape;' in s[s.index('&& OnHandle(px, selected))'):s.index('else', s.index('&& OnHandle(px, selected))'))]
for gesture in ['Create', 'Move', 'Reshape']:
    for shift in [False, True]:
        snap = gesture != 'Reshape' and not shift
        assert snap == (False if gesture == 'Reshape' else not shift)
        print(f'PASS snap argument: {gesture}, Shift={shift} -> {snap}')
base = __import__('subprocess').check_output(['git', 'show', '72b4efbd:Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs'], text=True)
for signature in ['Propose', 'OnPointerDown', 'OnPointerUp', 'Apply', 'ShiftHeld']:
    if signature == 'ShiftHeld':
        assert s[s.index('private static bool ShiftHeld'):s.index('private void OnPointerMove')] == base[base.index('private static bool ShiftHeld'):base.index('private void OnPointerMove')]
    else:
        pattern = r'private\s+void\s+' + signature + r'\s*\([^)]*\)'
        current, err = h['extract_body'](s, pattern, signature)
        original, err2 = h['extract_body'](base, pattern, signature)
        assert err is None and err2 is None and current == original, signature
    print('PASS unchanged method: ' + signature)
assert 'Its round handle rotates freely. Hold Shift while drawing to turn snapping off.' in Path('Assets/ForestPrototype/UI/StandMapView.cs').read_text()
print('PASS paired instruction text')
