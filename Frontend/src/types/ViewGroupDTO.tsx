export type GroupMemberDTO = {
  id: number;
  name: string;
  balance: number;
};

export type TransactionDTO = {
  id: number;
  description: string;
  amount: number;
  createdBy: string;
};

export type ViewGroupDTO = {
  id: number;
  title: string;
  members: GroupMemberDTO[];
  transactions: TransactionDTO[];
};

