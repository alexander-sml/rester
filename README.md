# RESTer - Cross-platform HTTP client CLI tool for .NET

[![.NET](https://img.shields.io/badge/.NET-6.0-purple.svg)](https://dotnet.microsoft.com)
[![License: CC BY-NC 4.0](https://img.shields.io/badge/License-CC_BY--NC_4.0-blue.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/platform-Windows%20%7C%20Linux%20%7C%20macOS-green.svg)]()

A modern, cross-platform alternative to curl and wget, built with .NET 6+.
Simple syntax, JSON support, and developer-friendly features out of the box.

## ✨ Features

- ✅ **Cross-platform** - Windows, Linux, macOS
- ✅ **Simple syntax** - `rest get https://api.example.com`
- ✅ **Multiple authentication methods** - Bearer tokens, Basic Auth, OAuth2
- ✅ **File upload/download** - With progress indicators
- ✅ **Timeout & retry** - Configurable request policies

## 📦 Installation

### As global .NET tool:
```bash
dotnet tool install --global rester
```

## 🚀 Quick Examples

# Simple GET request
rest get https://jsonplaceholder.typicode.com/posts/1

# POST with JSON data
rest post https://api.example.com/users \
  -h "Content-Type: application/json" \
  -d '{"name": "John", "email": "john@example.com"}'

# With authentication headers
rest get https://api.github.com/user \
  -h "Authorization: Bearer ghp_your_token" \
  -h "Accept: application/vnd.github.v3+json"

# Download file with progress
rest get https://example.com/file.zip -o ./download.zip --progress

## 📚 Command Reference
USAGE:
rest [METHOD] URL [OPTIONS]

METHOD:
get, post, put, delete, patch, head, options (default: get)

OPTIONS:
-X, --request      Set HTTP method
-H, --header       Add HTTP header (format: "Key: Value")
-d, --data         Request body data
-o, --output       Write response to file
-v, --verbose      Show detailed request/response info
--connect-timeout  Request timeout in seconds (default: 30)
--max-time         Total timeout in seconds (default: 60)
-r, --retry        Number of retries on failure
-k, --insecure     Allow insecure SSL connections
-L, --location     Follow redirects
--proxy            Use proxy server (example: https://user:password@example.com:2345)
