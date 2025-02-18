<#
  .SYNOPSIS
  Pushes containers to the container registry.

  .PARAMETER Tag
  Tag of the container image - e. g. 1.0-latest

  .PARAMETER RegistryUrl
  Hostname and path of the registry (without URI scheme) - e. g. myregistry.azurecr.io

  .PARAMETER RegistryUserName
  Username for the registry. Optional - if not specified, docker login will not be called.

  .PARAMETER RegistryPassword
  Password for the registry. Optional - if not specified, docker login will not be called.

  .EXAMPLE
  PS> .\push-containers.ps1 -Version latest -RegistryUrl myregistry.azurecr.io
#>

param(
  [Parameter(Mandatory)][String] $Tag,
  [Parameter(Mandatory)][String] $RegistryUrl,
  $RegistryUserName,
  $RegistryPassword
)

if ($RegistryUserName) {
  & docker login $RegistryUrl -u $RegistryUserName -p $RegistryPassword | out-host
}

& docker tag northwindstoreapp:latest $RegistryUrl/northwindstore/app:$Tag | out-host
& docker push $RegistryUrl/northwindstore/app:$Tag | out-host
