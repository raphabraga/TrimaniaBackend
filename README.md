<div id="top", align="center">

# Trimania Backend 🟣🛒

## API built for the frontend of Trimania E-commerce store website.

![license](https://img.shields.io/github/license/raphabraga/TrimaniaBackend)
![open issues](https://img.shields.io/github/issues/raphabraga/TrimaniaBackend?color=brightgreen)
![closed issues](https://img.shields.io/github/issues-closed/raphabraga/TrimaniaBackend)
![repo size](https://img.shields.io/github/repo-size/raphabraga/TrimaniaBackend)
![total lines](https://img.shields.io/tokei/lines/github/raphabraga/TrimaniaBackend)

</div>

---

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#project-status">Project Status</a></li>
    <li><a href="#features">Features</a></li>
    <li><a href="#prerequisites">Prerequistes</a></li>
    <li><a href="#running-the-api">Running the API</a></li>
    <li><a href="#api-usage">API usage</a></li>
    <li><a href="#technologies">Technologies</a></li>
    <li><a href="#author">Author</a></li>
    <li><a href="#license">License</a></li>
  </ol>
</details>

---

### Project Status

    🚧  Trimania Backend project is still under development with few open issues, however its core features were tested and are working great.  🚧
    
<p align="right">(<a href="#top">back to top</a>)</p>

---

### Features

- [x] User registration
- [x] User authentication/authorization
- [x] Product registration
- [x] Product selling (sales cart)
- [x] Sales report generation

<p align="right">(<a href="#top">back to top</a>)</p>

---

### Prerequisites

- Before you are able to run the API, you should have [Git](https://git-scm.com),
  [Docker Engine](https://docs.docker.com/engine/install/) and [Docker Compose](https://docs.docker.com/compose/install/)
  installed in any machine which gives support to those tools. Besides that, the docker service must be started.
  
<p align="right">(<a href="#top">back to top</a>)</p>

---

### Running the API

```Bash
# Clone the repository

$ git clone <https://github.com/raphabraga/TrimaniaBackend.git>

# Access the project folder

$ cd TrimaniaBackend/

# Build the docker containers

$ docker-compose -f docker-compose.prod.yml build

# Run the docker containers

$ docker-compose -f docker-compose.prod.yml up -d

# The backend will run in a docker container listening to TCP port 80 (accessed at <http://localhost/api/v1>).
# There is also a MySQL database running in another container which listen to TCP port 3306.
```

<p align="right">(<a href="#top">back to top</a>)</p>

---

### API usage

#### Version 1.0

The API follows REST architecture and it has the following endpoints:

- [x] /api/v1/login/
- [x] /api/v1/users/
- [x] /api/v1/products/
- [x] /api/v1/orders/
- [x] /api/v1/orders-summary/

For further details about each endpoints, you can access the swagger documentation at localhost/swagger/index.html.

For testing purposes, the MySQL database which serves the backend, it is populated with information like admin user, customer users and products.

The admin (login: admin, password: #tr1l0g0) has elevated privileges in the API, it is allowed to perform operations e.g. delete user, register new product, get all users order report, etc. 

<p align="right">(<a href="#top">back to top</a>)</p>

---

### Technologies

The following tools were used to build the project:

- [ASP.NET Core 6.0](https://docs.microsoft.com/pt-br/aspnet/core/?view=aspnetcore-6.0)
- [Entity Framework Core](https://docs.microsoft.com/pt-br/ef/core/)
- [FluentValidation](https://fluentvalidation.net)
- [BcryptNet](https://github.com/BcryptNet/bcrypt.net)
- [AutoMapper](https://automapper.org)
- [xUnit](https://xunit.net)
- [Moq](https://github.com/Moq/moq4/wiki/Quickstart)
- [MySQL](https://dev.mysql.com/doc/)
- [Docker](https://docs.docker.com)

<p align="right">(<a href="#top">back to top</a>)</p>

---

### Author

<a href="https://www.linkedin.com/in/raphael-braga-ev/">
 <img style="border-radius: 50%;" src="https://avatars.githubusercontent.com/raphabraga" width="100px;" alt=""/>
 <br />
 <sub><b>Raphael Braga Evangelista</b></sub>
</a>

Spiral out. Keep going!

Let's talk!

[![Linkedin Badge](https://img.shields.io/badge/-Raphael-blue?style=flat-square&logo=Linkedin&logoColor=white&link=https://www.linkedin.com/in/raphael-braga-ev/)](https://www.linkedin.com/in/raphael-braga-ev/)
[![Gmail Badge](https://img.shields.io/badge/-raphaelbraga.br@gmail.com-c14438?style=flat-square&logo=Gmail&logoColor=white&link=mailto:raphaelbraga.br@gmail.com)](mailto:raphaelbraga.br@gmail.com)

<p align="right">(<a href="#top">back to top</a>)</p>

---

### License

Distributed under the MIT License. See LICENSE.txt for more information.

<p align="right">(<a href="#top">back to top</a>)</p>
