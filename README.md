ReadMe for MicroPay app

# Description

The MicroPay app consists of two Microservices - Accounts and Transactions. These two services are decoupled and have their own databases. In accounts, we are able to create new accounts, set a start balance, and choose if the account is allowed to overdraft. We are also able to get the current balance of the account. Whenever a call is made to Deposit or Withdraw, if successful, this will send a transaction over to the Transactions api to be stored. The Transaction api also has an endpoint to retrieve the latest 10 transactions.

# Setup and run details

1. Clone repository and open microPay.sln in Visual Studio
2. To run all 3 projects simultaneously (Accounts, Transactions, Gateway), right click on solution and pick "Configure Startup Projects", set all 3 projects that are not tests to "Start" or "Start without Debugging".
3. To run all unittests + integration test, stop running projects, then right click on "microPay.Transactions" -> Debug -> Start Without Debugging, then do Ctrl+R followed by A, to run all tests (Transactions microservice needs to be running for the automatic integration test to succeed).

# Cloud Database

The project has been setup to connect to a Google Cloud Db.

# Swagger

Swagger has been enabled for both projects so this can be used to interact with the app. The enpoints available are

### Accounts
GetBalanceByUsername
CreateAccount
Deposit
Withdraw

### Transactions
CreateTransaction
GetLatestTransactionsByUsername

# Ocelot gateway

To manage calls in a centralized way, with the Gateway app running togethter with Accounts and Transactions, intstead of having to call for example

https://localhost:2000/Accounts/Deposit (Accounts)

https://localhost:3000/Transactions/GetLatestTransactionsByUsername?username={username} (Transactions)

It allows to use one entrypoint for all methods in Accounts and Transactions like

https://localhost:1000/Deposit

https://localhost:1000/GetLatestTransactionsByUsername?username={username}

Ocelot allows for rate limiting and caching, though these features have not been activated.

# Development process

The app has been developed with TDD principles, in phases described below. This means that unittests for various functionalities has been developed first, and then features to pass the test second, followed by refactoring and smaller changes. Specific usecases were first thought out, then the controllers were developed, and lastly the services to perform the actions. Below is described the ordering in which development was performed.

1. Setup and minimal architecture
---------------
2. AccountsController - Unittests
3. AccountsController - Features
---------------
4. AccountsService - Unittests
5. AccountsService - Features
---------------
6. TransactionsController - Unittests
7. TransactionsController - Features
---------------
8. TransactionsService - Unittests
9. TransactionsService - Features
---------------
10. Integration test
11. AccountsService - Send transactions to Transaction service

The lines above mark possible splitting into userstories/assignments, and one of the key positives about developing in microservice architecture is that multiple teams can work seperately in parallel on the two api's.

# ToDo's

The features below was not created as part of this project but would be needed in the future:

1. Implement authorization and authentication - JWT token/ Basic Auth, and specific roles, admin/users etc.
2. Microservices could be set up to communicate via a message broker RabbitMQ/Kafka etc. to set up a more distributed system that can be deployed in containers.