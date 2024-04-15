# Customer service campaign API Documentation

## Application Overview
A huge telecommunication company had a successful year and just recently started a new campaign to
reward their loyal customers (provided) with a discount on new purchases. Each day for one week
agents have to fill out the custom form to reward some of them with a daily limit of 5 customers per
agent. Mistakes are possible.

One month after this campaign they will get a .csv report with customers that have made a successful
purchase and they want to merge this data to show the results through API.
Since they want to reuse this scenario in different CRM solutions they have, securely exposed APIs are
required with ease of integration.

User service for customer data (FindPerson): https://www.crcind.com/csp/samples/SOAP.Demo.cls

## API Endpoints

### Agent Operations
- **Get All Agents**
  - **Endpoint**: `GET /api/Agent`
  - **Authorization**: Required
  - **Description**: Retrieves a list of all agents.
  - **Response Example**:
  ```JSON
    [
        {
        "firstName": "Marko",
        "lastName": "Markovic",
        "email": "markomarkovic@gmail.com"
        },
        {
        "firstName": "Ivan",
        "lastName": "Ivanovic",
        "email": "ivanivanovic@gmail.com"
        },
        {
        "firstName": "Nikola",
        "lastName": "Nikolic",
        "email": "nikolanikolic@gmail.com"
        }
    ]
  ```

- **Get Agent By ID**
  - **Endpoint**: `GET /api/Agent/{id}`
  - **Authorization**: Required
  - **Description**: Fetches details of a specific agent using their ID.
  - **Response Example**:
 ```JSON
    {
      "firstName": "Marko",
      "lastName": "Markovic",
      "email": "markomarkovic@gmail.com"
    }
```

### Authentication
- **Login**
  - **Endpoint**: `POST /api/Authentication/login`
  - **Authorization**: Not Required
  - **Description**: Authenticates agents by their email and password, issues JWT for sessions.
  - **Request Example**:
  ```JSON
    {
      "email": "agent@example.com",
      "password": "pass1234"
    }
  ```
  - **Response Example**:
  ```JSON
    {
      "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    }
  ```

### Campaign Management
- **Get Campaigns**
  - **Endpoint**: `GET /api/Campaign`
  - **Authorization**: Required
  - **Description**: Lists all campaigns.
  - **Response Example**:
  ```
    [
      {
        "campaignName": "test kampanja",
        "startDate": "2024-04-13T13:32:34.971",
        "endDate": "2024-04-19T13:32:34.971"
      },
      {
        "campaignName": "kampanja 2",
        "startDate": "2024-02-11T20:20:20.507",
        "endDate": "2024-02-17T20:20:20.507"
      },
      {
        "campaignName": "kampanja 3",
        "startDate": "2024-01-01T20:26:23.908",
        "endDate": "2024-01-07T20:26:23.908"
      },
      {
        "campaignName": "kampanja 44",
        "startDate": "2024-04-15T08:06:59.527",
        "endDate": "2024-04-21T08:06:59.527"
      }
    ]
  ```

- **Get Campaign By ID**
  - **Endpoint**: `GET /api/Campaign/{id}`
  - **Authorization**: Required
  - **Description**: Retrieves detailed information about a specific campaign.
  ```JSON
    {
      "email": "agent@example.com",
      "password": "pass1234"
    }
  ```

- **Create Campaign**
  - **Endpoint**: `POST /api/Campaign`
  - **Authorization**: Required
  - **Description**: Adds a new campaign to the system.
  - **Request Example**:
    ```JSON
    {
      "campaignName": "Summer Rewards Test",
      "startDate": "2024-04-15T08:06:59.527Z"
    }
    ```
  - **Response Example**:
    ```JSON
    {
      "campaignName": "Summer Rewards Test",
      "startDate": "2024-04-15T08:06:59.527Z",
      "endDate": "2024-04-21T08:06:59.527Z"
    }
    ```

- **Delete Campaign**
  - **Endpoint**: `DELETE /api/Campaign/{id}`
  - **Authorization**: Required
  - **Description**: Deletes an existing campaign from the system.

### Purchase Management
- **Get Purchases**
  - **Endpoint**: `GET /api/Purchase`
  - **Authorization**: Required
  - **Description**: Retrieves all purchases.

- **Get Purchase By ID**
  - **Endpoint**: `GET /api/Purchase/{id}`
  - **Authorization**: Required
  - **Description**: Provides details of a specific purchase.

- **Create Purchase**
  - **Endpoint**: `POST /api/Purchase/create`
  - **Authorization**: Required
  - **Description**: Submits a new purchase entry, including agent ID, customer ID, campaign ID, price, and discount.
  - **Request Example**:
    ```JSON
    {
      "agentId": 2,
      "customerId": 7,
      "campaignId": 32,
      "price": 100.99,
      "discount": 20
    }
    ```
  - **Response Example**:
    ```JSON
    {
      "id": 58,
      "agentId": 2,
      "customerId": 7,
      "campaignId": 32,
      "price": 100.99,
      "discount": 20.0,
      "priceWithDiscount": 80.792,
      "purchaseDate": "2023-10-22T19:11:18.042Z"
    }
    ```
- **Delete Purchase**
- **Endpoint**: `DELETE /api/Purchase/{id}`
- **Authorization**: Required
- **Description**: Deletes a purchase by its ID.

### Customer Management
- **Get Customer Information by ID
  - **Endpoint**: `GET /api/Purchase/customer/{customerId}`
  - **Authorization**: Required
  - **Description**: Retrieves customer information by customer ID, utilizing an external SOAP service.
   ```JSON
     {
       "id": 3,
       "name": "John Doe",
       "ssn": "123-45-6789",
       "dob": "1980-01-01",
       "homeAddress": {
       "street": "1234 Elm St",
       "city": "Somewhere",
       "state": "CA",
       "zip": "90210",
            "officeAddress": {
              "street": "5678 Maple St",
              "city": "Anywhere",
              "state": "NY",
              "zip": "10001"
            },
      "age": 42
     }      
    ```
- **Customer Existence Check**
    - **Endpoint**: `GET /api/Purchase/customer-exists/{customerId}`
    - **Authorization**: Required
    - **Description**: Checks if a customer exists using an external SOAP service.


### Reporting
- **Export Campaign Purchases to CSV**
  - **Endpoint**: `GET /api/Campaign/export/{campaignId}`
  - **Authorization**: Required
  - **Description**: Generates a CSV report detailing all purchases associated with a specific campaign.
  - **Response**: Returns a CSV file with detailed purchase information.
  