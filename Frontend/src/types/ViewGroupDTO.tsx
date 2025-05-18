export type GroupMemberDTO = {
  id: number;
  name: string;
  balance: number;
};

export type TransactionDTO = {
  id: number;
  amount: number; 
  description: string;
  date: Date;
  userId : number;
  userName: string;
  groupId: number;
};

export type ViewGroupDTO = {
  id: number;
  title: string;
  members: GroupMemberDTO[];
  transactions: TransactionDTO[];
};

export type SettleDTO = {
  splitId: number;
  userName: string;
  amount: number;
};