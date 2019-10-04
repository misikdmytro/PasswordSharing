# Password Sharing

## How to run project
1. Open solution folder;
2. Run 'docker-compose up web' 

## How to run unit tests
1. Open solution folder
2. Run 'docker-compose up unit'

## How to run intefration tests
1. Open solution folder
2. Run 'docker-compose up api'

## Swagger endpoint
Project contains swagger page located [here](http://localhost:8080/swagger)

## Elasticsearch
Elasticsearch collects logs. Located [here](http://localhost:9200)

## Kibana
Kibana located [here](http://localhost:5601)
	
## API description:
API described on page http://localhost:8080/swagger when application started
There are 2 API methods:
1. Generate passwords:
	- Description: method used to encode and share passwords. Returns link to passwords.
	- Method: POST
	- URL: 'api/password'
	- Body: { "password": \["string"\], "expiresIn": "int32" }
	- OK result: 200 OK
	- Fail result: 400 Bad Request; 500 Internal Server Error
2. Retrieve passwords:
	- Description: method used to retrieve passwords by shared link.
	- Method: GET
	- URL: 'api/password/{passwordId}?key={key}'
	- OK Result: 200 OK
	- Fail result: 400 Bad Request; 500 Internal Server Error

## Architecture
For development used .NET Core 2.1 with Redis server.
Logs uses Elasticsearch and Kibana.
For password encription used RSA algorithm. Key size is 1024. 
So max password size that can be encrypted is 117 bytes.
New encryption key generates each time new password encrypted.
Decryption key passed via share URL. This is the only place used for key storing.
Password randomly re-encrypts after is was decrypted or expired without possibility to decrypt.
This guarantees that retrieved password will never decrypted again.
It's impossible to decrypt password because of key storing only in URL and re-encryption password after use it.
	
## Details:
- every time on password encryption new key generated.
- the only place where key stored is URL.
- every time unique URL generated even for same passwords
- it is impossible to decrypt used passwords.
- logging enabled (HTTP requests/responses, atomic operations, request details, etc.)