<div align="center">

# Trimania Backend

## API built for the frontend of Trimania E-commerce store website.

![license](https://img.shields.io/github/license/raphabraga/TrimaniaBackend)
![open issues](https://img.shields.io/github/issues/raphabraga/TrimaniaBackend?color=brightgreen)
![closed issues](https://img.shields.io/github/issues-closed/raphabraga/TrimaniaBackend)
![repo size](https://img.shields.io/github/repo-size/raphabraga/TrimaniaBackend)
![total lines](https://img.shields.io/tokei/lines/github/raphabraga/TrimaniaBackend)

</div>

# Table of contents

<!--ts-->

- [Project Status](#project-status)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Running the API](#running-the-api)
- [Technologies](#technologies)
- [Author](#author)
- [License](#license)
<!--te-->

### Project Status

    🚧  Trimania Backend project is still under development with few open issues, however its core features were tested and are working great.  🚧

### Features

- [x] User registration
- [x] User authentication/authorization
- [x] Product registration
- [x] Product selling (sales cart)
- [x] Sales report generation

### Prerequisites

- Before you are able to run the API, you should have [Git](https://git-scm.com) and
  [Docker Engine](https://docs.docker.com/engine/install/) and [Docker Compose](https://docs.docker.com/compose/install/) 
  installed in any machine which gives support to those tools. Besides that the docker service should be started.

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






