"""Compatibility entry for current APK preflight; offline unless --device is explicit."""
from pathlib import Path
import runpy

if __name__ == '__main__':
    current = Path(__file__).resolve().parents[2] / 'R2OfflineHandoff/Sources/Preflight.py'
    runpy.run_path(str(current), run_name='__main__')
