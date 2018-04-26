# nice-nuget-server

A nice nuget server implementation able to use a variety of different package providers and caching stategies.

## Configuration

|Key         |Description                      |
|------------|---------------------------------|
|`Aws:Client`|Client id for accessing S3       |
|`Aws:Secret`|Client secret for accessing S3   |
|`Aws:Bucket`|The bucket packages are stored in|
|`BaseUrl`   |The base url of this application |

## Usage

Add the following line to your **NuGet.Config** file:

`<add key="Nice NuGet" value="{BaseUrl}/v2" protocolVersion="2" />`

*Adapted from https://github.com/TanukiSharp/MinimalNugetServer*