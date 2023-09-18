# CourseLibrary API

CourseLibrary REST API built in .NET 6.

## Dependencies

- .NET 6
- [Microsoft.EntityFrameworkCore](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/6.0.6)
- [Microsoft.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/6.0.6)
- [Microsoft.EntityFrameworkCore.Tools](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tools/6.0.6)
- [Microsoft.AspNetCore.Mvc.NewtonsoftJson](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.NewtonsoftJson/)
- [Microsoft.AspNetCore.JsonPatch](https://www.nuget.org/packages/Microsoft.AspNetCore.JsonPatch/7.0.10)
- [AutoMapper](https://www.nuget.org/packages/AutoMapper.Extensions.Microsoft.DependencyInjection/8.1.1)

---

## API documentation

- [Get Authors](#get-authors)
  - [Get Authors Request](#get-authors-request)
  - [Get Authors Response](#get-authors-response)
- [Get Author](#get-author)
  - [Get Author Request](#get-author-request)
  - [Get Author Response](#get-author-response)
- [Create Author](#create-author)
  - [Create Author Request](#create-author-request)
  - [Create Author Response](#create-author-response)
- [Get Authors Options](#get-authors-options)
  - [Get Authors Options Request](#get-authors-options-request)
  - [Get Authors Options Response](#get-authors-options-response)

## Get Authors

### Get Authors Request

```js
GET /api/authors
```

or with request query string optional parameters

```js
GET /api/authors?mainCategory={{category}}&q={{searchQuery}}&pageNumber={{pageNum}}&pageSize={{pageSize}}
```

### Get Authors Response

```js
200 Ok
```

```json
[
    {
        "id": "102b566b-ba1f-404c-b2df-e2cde39ade09",
        "name": "Arnold The Unseen Stafford",
        "age": 66,
        "mainCategory": "Singing"
    }
]
```

## Get Author

### Get Author Request

```js
GET /api/authors/{{id}}
```

### Get Author Response

```js
200 Ok
```

```json
{
    "id": "102b566b-ba1f-404c-b2df-e2cde39ade09",
    "name": "Arnold The Unseen Stafford",
    "age": 66,
    "mainCategory": "Singing"
}
```

## Create Author

### Create Author Request

```js
POST /api/authors
```

```json
{
	"firstName" : "Jane",
	"lastName" : "Skewers",
	"dateOfBirth" : "1968-03-04T00:00:00",
	"mainCategory": "Rum"
}
```

### Create Author Response

```js
201 Created
```

```yml
Location: {{host}}/api/authors/{{id}}
```

```json
{
    "id": "2d594794-5ce3-485a-981f-7c62fadbdd82",
    "name": "Jane Skewers",
    "age": 55,
    "mainCategory": "Rum"
}
```

## Get Authors Options

### Get Authors Options Request

```js
OPTIONS /api/authors
```

### Get Authors Options Response

```js
200 Ok
```
Headers: (No body)

```json
{
    "Allow": "GET,HEAD,POST,OPTIONS",
    ...
}
```

---

## Acknowledgments

- [Kevin Dockx](https://github.com/KevinDockx)

## License

The software is available as open source under the terms of the [MIT License](https://opensource.org/licenses/MIT).
