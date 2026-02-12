# dotnet-web-simple application

It is a simple dotnet web application which uses `net8.0` as a target framework.

 
# Application Output

The app will print `Hello World!`


## Image Of Devfile

[Community .NET Image](quay.io/devspaces/dotnet-90) is used as an image for .NET development and it's defined in https://github.com/devspaces-samples/dotnet-web-simple/blob/devspaces-3-rhel-9/workspace-container-image/Containerfile.


Running MS SQL Server in an OpenShift Dev Spaces workspace

Microsoft SQLServer requires a "privileged" port, (port number < 1024), in order to run.  Port 135.

So, we need to modify the default Dev Spaces SCC to allow a container to ask for the `NET_BIND_SERVICE` capability.


Create the new SCC.

```bash
cat << EOF | oc apply -f -
apiVersion: security.openshift.io/v1
kind: SecurityContextConstraints
metadata:
  name: custom-devspaces-scc
allowHostDirVolumePlugin: false
allowHostIPC: false
allowHostNetwork: false
allowHostPID: false
allowHostPorts: false
allowPrivilegeEscalation: true
allowPrivilegedContainer: false
allowedCapabilities:
- SETUID
- SETGID
- NET_BIND_SERVICE
fsGroup:
  type: MustRunAs
groups: []
readOnlyRootFilesystem: false
runAsUser:
  type: MustRunAsRange
seLinuxContext:
  type: MustRunAs
supplementalGroups:
  type: RunAsAny
users: []
volumes:
- configMap
- downwardAPI
- emptyDir
- persistentVolumeClaim
- projected
- secret
EOF
```

Modify your `CheCluster` custom resource to use the new SCC.

YAML snippet below -

```yaml
apiVersion: org.eclipse.che/v2 
kind: CheCluster
...
...
spec:                         
  devEnvironments:       
    containerBuildConfiguration:
      openShiftSecurityContextConstraint: custom-devspaces-scc
    disableContainerBuildCapabilities: false
...
```

Create a workspace from this code repo - `https://github.com/cgruver/devspaces-sqlserver-workspace.git`

The relevant section in the `devfile.yaml` that makes SQL Server work is the `container-overrides` in the container component definition for the SQL Server container.

```yaml
- name: sqlserver-container
  attributes:
    container-overrides: 
      securityContext:
        capabilities:
          add:
            - NET_BIND_SERVICE
  container:
    image: mcr.microsoft.com/mssql/rhel/server:2022-CU17-rhel-9.1      
```

