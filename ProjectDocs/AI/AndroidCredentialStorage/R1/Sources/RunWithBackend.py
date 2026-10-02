"""Run Unity verification against an isolated real loopback HTTP backend. No credentials logged."""
from pathlib import Path
import json, os, subprocess, sys, tempfile, threading
root=Path(__file__).resolve().parents[5]
base=Path(__file__).resolve().parent
sys.path.insert(0,str(root/'Backend/LegendsOfRusService'))
from service import Application,Service
from rustore import RuStoreVerifier
from run_local import QuietHandler
from wsgiref.simple_server import make_server
with tempfile.TemporaryDirectory(prefix='lor-backend-verification-') as folder:
    app=Application(Service(Path(folder)/'state.db',RuStoreVerifier('123','sandbox',lambda:'')))
    server=make_server('127.0.0.1',0,app,handler_class=QuietHandler)
    thread=threading.Thread(target=server.serve_forever,daemon=True);thread.start()
    config=base/'Loopback.json'
    overrides={'LOR_BACKEND_TEST_ORIGIN':'http://127.0.0.1:'+str(server.server_port)}
    if 'ContinuationTransaction' in sys.argv[2]:
        overrides.update(XDG_CONFIG_HOME='/tmp/lor-continuation-r1/AndroidCredentials-'+sys.argv[1]+'/config', LOR_LOADING_TEST_CONFIG_ROOT='/tmp/lor-continuation-r1/AndroidCredentials-'+sys.argv[1]+'/config')
    config.write_text(json.dumps(overrides))
    try:
        result=subprocess.run([sys.executable,str(base/'Verify.py'),sys.argv[1],'PlayMode',sys.argv[2],str(config)],env={**os.environ,'PYTHONDONTWRITEBYTECODE':'1'})
        sys.exit(result.returncode)
    finally:
        server.shutdown();thread.join(timeout=3);server.server_close()
