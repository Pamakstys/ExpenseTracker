import { useEffect, useState } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Typography,
  Stack,
  CircularProgress,
} from "@mui/material";
import type { TransactionDTO } from "../types/ViewGroupDTO";

interface ViewTransactionModalProps {
  open: boolean;
  onClose: () => void;
  groupId: string;
  onSuccess: () => void;
  transactionId: number | null;
}

export default function ViewTransactionModal({
  open,
  onClose,
  transactionId,
}: ViewTransactionModalProps) {
  const [transaction, setTransaction] = useState<TransactionDTO | null>(null);
  const [loading, setLoading] = useState(false);
  const apiUrl = import.meta.env.VITE_API_URL;
  const userId = localStorage.getItem("id");

  useEffect(() => {
    if (open && transactionId) {
      fetchTransaction();
    }
  }, [open, transactionId]);

  const fetchTransaction = async () => {
    setLoading(true);
    try {
      const response = await fetch(`${apiUrl}/transaction/view`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ UserId: userId, TransactionId: transactionId }),
      });

      if (response.ok) {
        const data = await response.json();
        setTransaction(data);
      } else {
        console.error("Failed to fetch transaction");
      }
    } catch (err) {
      console.error("Error fetching transaction:", err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>Transaction Details</DialogTitle>
      <DialogContent>
        {loading ? (
          <Stack alignItems="center" py={2}>
            <CircularProgress />
          </Stack>
        ) : transaction ? (
          <Stack spacing={2} py={1}>
            <Typography>
              <strong>Amount:</strong> €{transaction.amount.toFixed(2)}
            </Typography>
            <Typography>
              <strong>Description:</strong> {transaction.description}
            </Typography>
            <Typography>
              <strong>Date:</strong>{" "}
              {new Date(transaction.date).toLocaleDateString()}
            </Typography>
            <Typography>
              <strong>Created By:</strong> {transaction.userName}
            </Typography>
          </Stack>
        ) : (
          <Typography>No transaction found.</Typography>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} variant="contained">
          Close
        </Button>
      </DialogActions>
    </Dialog>
  );
}
