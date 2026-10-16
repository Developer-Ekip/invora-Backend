"""Çalışan API için uçtan uca test. Yalnızca ayrı test veritabanında çalıştırın.
FIELDOPS_TEST_URL, FIELDOPS_ADMIN_USERNAME, FIELDOPS_ADMIN_PASSWORD gerektirir.
"""
import json
import os
import urllib.request
import urllib.error
import uuid

base = os.environ['FIELDOPS_TEST_URL'].rstrip('/')
count = 0

def request(method, path, expected, body=None, token=None):
    global count
    headers = {'Content-Type': 'application/json'}
    if token:
        headers['Authorization'] = 'Bearer ' + token
    req = urllib.request.Request(base + path, data=json.dumps(body).encode() if body is not None else None, headers=headers, method=method)
    try:
        response = urllib.request.urlopen(req)
    except urllib.error.HTTPError as error:
        response = error
    data = response.read()
    assert response.status == expected, f'{method} {path}: expected {expected}, got {response.status}: {data[:300]}'
    count += 1
    return json.loads(data) if data and response.headers.get('Content-Type', '').startswith('application/json') else data

suffix = uuid.uuid4().hex[:10]
admin = request('POST', '/api/auth/login', 200, {'username': os.environ['FIELDOPS_ADMIN_USERNAME'], 'password': os.environ['FIELDOPS_ADMIN_PASSWORD']})['token']
users = []
for n in range(2):
    credentials = {'username': f'test-{suffix}-{n}', 'password': 'TestPassword123!'}
    user = request('POST', '/api/auth/register', 201, credentials)
    token = request('POST', '/api/auth/login', 200, credentials)['token']
    users.append((user['id'], token))
request('POST', '/api/auth/register', 409, credentials)
request('POST', '/api/auth/login', 401, {'username': credentials['username'], 'password': 'wrong'})
a, b = users[0][1], users[1][1]
request('GET', '/api/visits', 401)
request('GET', '/api/visits', 401, token='invalid')
request('POST', '/api/companies', 403, {'name': 'Denied'}, a)
request('POST', '/api/companies', 400, {'name': '   '}, admin)
company = request('POST', '/api/companies', 201, {'name': 'Test ' + suffix}, admin)
site = request('POST', '/api/sites', 201, {'name': 'Test Site', 'companyId': company['id']}, admin)
visit_body = {'siteId': site['id'], 'assignedUserId': users[0][0], 'visitDate': '2026-10-20T12:00:00Z', 'status': 'Planned'}
request('POST', '/api/visits', 403, visit_body, a)
request('POST', '/api/visits', 400, {**visit_body, 'assignedUserId': 2147483647}, admin)
request('POST', '/api/visits', 400, {**visit_body, 'status': 'Unknown'}, admin)
request('POST', '/api/visits', 400, {k: v for k, v in visit_body.items() if k != 'visitDate'}, admin)
visit = request('POST', '/api/visits', 201, visit_body, admin)
vid = visit['id']
request('GET', f'/api/visits/{vid}', 200, token=a)
request('GET', f'/api/visits/{vid}', 404, token=b)
assert request('GET', '/api/visits', 200, token=b) == []
assert request('GET', '/api/sites', 200, token=b) == []
assert request('GET', '/api/companies', 200, token=b) == []
request('PATCH', f'/api/visits/{vid}/status', 404, {'status': 'Completed'}, b)
request('PATCH', f'/api/visits/{vid}/status', 400, {'status': 'Unknown'}, a)
request('PATCH', f'/api/visits/{vid}/status', 204, {'status': 'InProgress'}, a)
for route, initial, done in [('fieldtasks', 'Pending', 'Completed'), ('issues', 'Open', 'Resolved')]:
    body = {'visitId': vid, 'title': 'Test', 'description': 'Test record', 'status': initial}
    request('POST', '/api/' + route, 400, body, b)
    item = request('POST', '/api/' + route, 201, body, a)
    path = f'/api/{route}/{item["id"]}'
    request('GET', path, 404, token=b)
    assert request('GET', '/api/' + route, 200, token=b) == []
    request('PUT', path, 400, {**body, 'status': done}, b)
    request('DELETE', path, 404, token=b)
    request('PUT', path, 204, {**body, 'status': done}, a)
    request('PUT', path, 400, {**body, 'status': 'Invalid'}, a)
    request('GET', path, 200, token=admin)
    request('DELETE', path, 204, token=a)
request('PUT', f'/api/visits/{vid}', 204, {**visit_body, 'assignedUserId': users[1][0]}, admin)
request('GET', f'/api/visits/{vid}', 404, token=a)
request('GET', f'/api/visits/{vid}', 200, token=b)
assert all('passwordHash' not in u for u in request('GET', '/api/users', 200, token=admin))
request('GET', '/api/users', 403, token=a)
request('DELETE', f'/api/visits/{vid}', 403, token=b)
request('DELETE', f'/api/visits/{vid}', 204, token=admin)
request('DELETE', f'/api/sites/{site["id"]}', 204, token=admin)
request('DELETE', f'/api/companies/{company["id"]}', 204, token=admin)
spec = request('GET', '/swagger/v1/swagger.json', 200)
assert 'Bearer' in spec['components']['securitySchemes']
request('GET', '/swagger/index.html', 200)
print(f'PASS: {count} HTTP checks; assignment, isolation, validation, CRUD, JWT and Swagger.')
