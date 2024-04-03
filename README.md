# Backend-CRUD-React-18-dotnet-7
Boilerplate Backend CRUD Web App created with React 18 + .NET 7 by [StackPuz](https://stackpuz.com).

## Demo
Checkout the live demo at https://demo-spa.stackpuz.com

## Features
- Fully Responsive Layout.
- Sorting, Filtering and Pagination on Data List.
- User Management, User Authentication and Authorization, User Profile, Reset Password.
- Input Mask and Date Picker for date and time input field with Form Validation.

![Responsive Layout](https://stackpuz.com/img/feature/responsive.gif)  
![Data List](https://stackpuz.com/img/feature/list.gif)  
![User Module](https://stackpuz.com/img/feature/user.png)  
![Input Mask and Date Picker](https://stackpuz.com/img/feature/date.gif)

## Minimum requirements
- Node.js 14.18
- .NET 7
- MySQL 5.7

## Installation
1. Clone this repository. `git clone https://github.com/stackpuz/Backend-CRUD-React-18-dotnet-7.git .`
2. Change directory to React project. `cd Backend-CRUD-React-18-dotnet-7/react`
3. Install the React dependencies. `npm install`
4. Create a new database and run [/database.sql](/database.sql) script to create tables and import data.
5. Edit the database configuration in [/dotnet_api/appsettings.json](/dotnet_api/appsettings.json) file.
    ```
    "ConnectionStrings": {
        "Database": "server=127.0.0.1;port=3306;database=testdb;user id=root;password=;charset=utf8;"
    }
    ```

## Run project

1. Run React project. `npm run dev`
2. Run .NET project. `dotnet run --urls="http://localhost:5000"`
3. Navigate to `http://localhost:5173`
4. Login with user `admin` password `1234`

## Customization
To customize this project to use other Database Engines, CSS Frameworks, Icons, Input Mask, Date picker, Date format, Font and more. Please visit [stackpuz.com](https://stackpuz.com).

## License

[MIT](https://opensource.org/licenses/MIT)