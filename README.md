# gRPC on Microservices

A product service exposed over **gRPC**, with a console client that exercises every call type and a worker service that adds products on a timer.

> **Study project (2022).** Built while learning gRPC in .NET. See [How I would build it today](#how-i-would-build-it-today).

## Projects

| Project | Role |
|---|---|
| `ProductGrpc` | gRPC server: `ProductProtoService` over an EF Core InMemory database, with AutoMapper |
| `ProtoGrpcClient` | Console client that calls each RPC in turn |
| `ProductWorkerService` | Background worker that adds a generated product on a timer and keeps retrying while the server is unavailable |

## The contract

[`ProductGrpc/Protos/product.proto`](ProductGrpc/Protos/product.proto) covers the main gRPC call types:

| RPC | Type |
|---|---|
| `GetProduct`, `AddProduct`, `UpdateProduct`, `DeleteProduct` | Unary |
| `GetAllProducts` | Server streaming |
| `InsertBulkProduct` | Client streaming |

Missing products return `StatusCode.NotFound` through `RpcException`.

## Tech

.NET 5 · ASP.NET Core gRPC (`Grpc.AspNetCore`) · Protocol Buffers · EF Core InMemory · AutoMapper

## Running locally

```bash
dotnet run --project ProductGrpc      # server on https://localhost:5001
dotnet run --project ProtoGrpcClient  # runs the calls against the server
dotnet run --project ProductWorkerService  # adds a product every 3 seconds
```

The server uses HTTPS; create a local development certificate once with `dotnet dev-certs https --trust`.

On Apple Silicon, the old `Grpc.Tools` version can't detect the platform; build with `PROTOBUF_TOOLS_OS=macosx PROTOBUF_TOOLS_CPU=x64` (requires Rosetta) or upgrade `Grpc.Tools`.

The projects target .NET 5, which is out of support. To run them you need the .NET 5 runtime or `DOTNET_ROLL_FORWARD=Major`.

## How I would build it today

- **Client factory.** Register the client with `AddGrpcClient<ProductProtoServiceClient>()` from `Grpc.Net.ClientFactory` instead of creating channels by hand, and configure retries through a gRPC service config.
- **Deadlines on every call**, so a slow server can never hold a caller indefinitely.
- **Integration tests** that host the service in memory with `WebApplicationFactory` and cover each call type, including both streaming directions.
- **Current .NET and gRPC packages.** Newer `Grpc.Tools` versions also build natively on Apple Silicon.
