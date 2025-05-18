# User Group Management Web Application

This web application allows users to manage groups, create transactions, and split expenses among group members. It provides features for viewing group balances, adding/removing members, and settling debts within groups.

## Features

### 1. Groups
- View all your groups in one place.
- Create a new group by providing only a title.
- Displays a list of groups with the amount you owe or are owed in each group.
  
### 2. Group Details
- View all information about a specific group:
  - Group’s title.
  - A list of all members, with amounts owed or received.
  - Settling functionality for debts between members.
  - View a list of all transactions made within the group.
  
#### Member Management:
- Add new members to the group.
- Remove members only if they are settled with everyone in the group (i.e., no outstanding debts).

### 3. Transactions
- Create a new transaction:
  - Select the person who paid for the transaction.
  - Enter the total amount paid for the transaction.

#### Split Options:
- **Equally**: The amount is split equally between all members.
- **Percentage**: Enter a percentage for each member, and the system will calculate the amounts.
- **Dynamic**: Manually enter the exact amount each member owes, including yourself.

---

To run backend: dotnet run

To run frontend: 
npm install
npm run dev
