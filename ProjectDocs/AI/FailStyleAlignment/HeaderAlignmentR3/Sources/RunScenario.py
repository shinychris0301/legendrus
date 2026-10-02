from pathlib import Path
import sys
sys.path.insert(0,str(Path.cwd()/"Tools"))
import scenario_fail_r2 as batch
batch.BASE=Path(__file__).resolve().parent
batch.run(sys.argv[1])
