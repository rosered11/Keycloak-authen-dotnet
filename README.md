# Keycloak-authen-dotnet

Default url for connect keycloak

`http://localhost:8080`

## Warning

If you create a new account by default of keycloak, you must log in first, otherwise the token will not be generate.
And warning login to the wrong realm.

This url for login another realm

`http://localhost:8080/auth/realms/{my-realm}/account`

## Example Request

### Get Token

```
curl --location --request POST 'http://localhost:8080/auth/realms/my-realm/protocol/openid-connect/token' \
--header 'Content-Type: application/x-www-form-urlencoded' \
--data-urlencode 'grant_type=password' \
--data-urlencode 'client_id=demo-app' \
--data-urlencode 'client_secret=efb03c5b-7d90-4cb5-9c68-9cae606cdba2' \
--data-urlencode 'username=user' \
--data-urlencode 'password=P@ssword1'
```



## Reference

- [Keycloak REST API: Create a New User](https://www.appsdeveloperblog.com/keycloak-rest-api-create-a-new-user/)

- [keycloak-dotnetcore](https://github.com/vmaleze/keycloak-dotnetcore)

- [ASP.Net Core & Angular OpenID Connect using Keycloak](https://medium.com/@xavier.hahn/asp-net-core-angular-openid-connect-using-keycloak-6437948c008)

- [Adding authorization to Asp.Net Core app using Keycloak](https://medium.com/@xavier.hahn/adding-authorization-to-asp-net-core-app-using-keycloak-c6c96ee0e655)

- [Authorization Services Guide](https://www.keycloak.org/docs/latest/authorization_services/#_getting_started_overview)