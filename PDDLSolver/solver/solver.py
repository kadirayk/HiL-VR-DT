import json, sys
from urllib.request import Request
from urllib.request import urlopen

data = {'domain': open(sys.argv[1], 'r').read(),
        'problem': open(sys.argv[2], 'r').read()}

data = json.dumps(data).encode("utf-8")
req = Request('http://solver.planning.domains/solve')
req.add_header('Content-Type', 'application/json')
ans = urlopen(req, data=data)
resp = json.loads(ans.read().decode('utf-8'))

with open(sys.argv[3], 'w') as f:
    f.write('\n'.join([act['name'] for act in resp['result']['plan']]))
