# GS1 EPC Translator

GS1 EPC Translator is a project designed to parse translate EPCs between various GS1 formats: URN, DigitalLink, and ElementString. This project offers a RESTful API with a primary endpoint /translate that accepts an array of strings and returns an array of translated elements.

## Table of Contents

- Features
- Prerequisites
- Installation
- Usage
- API Endpoints
- POST /translate
- Contribution
- License

## Features
- Parse EPCs (Electronic Product Codes).
- Translate EPCs into three different GS1 formats:
  - URN
  - DigitalLink
  - ElementString
- Return results in JSON format.

The library aims to support all of these GS1 keys:
SGTIN, SSCC, GTIN, LGTIN, SGTIN, SGLN, GRAI, GIAI, GRSN, GSRNP, GDTI, CPI, SGC, GINC, GSIN, ITIP, UPUI, PGLN

## Prerequisites

This repository only requires .NET 8 SDK or later. There is no external dependencies to run the API.

##  Installation

1. Clone the GitHub repository:

```sh
git clone https://github.com/your-username/epc-translator-api.git
cd epc-translator-api
```

2. Restore packages and build the application:

```sh
dotnet restore
dotnet build
```

3. Run the application:

```sh
dotnet run
```

## Usage
You can use a tool like curl, Postman, or any other HTTP client to send requests to the API.

### Example request using curl

```sh
curl -X POST "http://localhost:5000/translate" -H "Content-Type: application/json" -d '["urn:epc:id:sgln:871933301053..0"]'
```

## API Endpoints

### POST /translate

- URL: /translate
- Method: POST
- Description: Accepts an array of strings representing EPCs and returns an array of elements translated into various GS1 formats.
- Parameters:
  - Body (JSON):
```json
[
  "urn:epc:id:sgln:871933301053..0",
  "urn:epc:id:sgtin:0614141.107346.2017"
]
```
- Responses:
  - 200 OK: Returns an array of translated objects.
  - 400 Bad Request: If the format of the EPCs is invalid.

Example response
```json
[
  {
    "type": "SGLN",
    "raw": "urn:epc:id:sgln:871933301053..0",
    "urn": "urn:epc:id:sgln:871933301053..0",
    "elementString": "(414)8719333010535",
    "digitalLink": "https://id.gs1.org/414/8719333010535"
  },
  {
    "type": "SGTIN",
    "raw": "urn:epc:id:sgtin:0614141.107346.2017",
    "urn": "urn:epc:id:sgtin:0614141.107346.2017",
    "elementString": "(01)00614141107346(21)2017",
    "digitalLink": "https://id.gs1.org/01/00614141107346/21/2017"
  }
]
```

## Contribution

Contributions are welcome! To report issues or propose enhancements, please open an issue or submit a pull request.

### Steps to contribute:

1. Fork this repository.
2. Create a branch for your feature (git checkout -b feature/my-feature).
3. Commit your changes (git commit -m 'Add my feature').
4. Push your branch (git push origin feature/my-feature).
5. Open a Pull Request.


## License
This project is licensed under the Apache 2.0 License. See the LICENSE file for more information.

For further questions or assistance, please contact me directly at fastnt@pm.me or ambroise.la@pm.me.
