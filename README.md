# 📄 Sistema de Cuentas por Pagar (CxP)

Este es un sistema web desarrollado en **ASP.NET Core MVC (.NET 8)** para la gestión de cuentas por pagar.  
Incluye administración de **Conceptos**, **Proveedores** y **Documentos por Pagar**, así como la generación de asientos contables enviados a una API.  
El sistema fue desplegado en **Azure**, almacenando tanto la base de datos como la aplicación en la nube.

---

## 🚀 Características

- Gestión de **Conceptos de Pago**
- Gestión de **Proveedores** con validación estricta de Cédula/RNC de República Dominicana
- Registro y consulta de **Documentos por Pagar**
- Generación automática de asientos contables y envío a API
- Despliegue en **Azure** (App Service + Base de Datos SQL en la nube)
- Conexión a base de datos mediante `SqlConnection` (sin Entity Framework)
- Interfaz con barra de navegación entre CRUDs

