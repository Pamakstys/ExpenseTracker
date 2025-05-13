A web application for managing user groups, allowing users to create transactions and split
expenses between the payer and other members within each group.

1. Groups
Page to view all your groups.
• You can create a new group.
• Displays a list of groups along with the amount you owe or are owed in each group.
• Creating a group only requires a title.

2. Group
Page to view all information about a specific group.
• Displays the group’s title.
• Shows all assigned members along with the amount you owe or are owed. If the amount is
not null, provides functionality to settle it.
• Allows viewing of transactions.
• Allows adding new members.
• Allows removing a member, but only if they are settled with everyone in the group.
• Allows creating a new transaction.

3. New Transaction
Page to create a new transaction.
• Select who paid for the transaction.
• Enter the full amount paid.

How to split the amount:
• Equally – Amount is split equally between all members.
• Percentage – Enter a percentage for each member; the system will convert it into amounts.
• Dynamic – Enter the exact amount for each member manually, including yourself.
