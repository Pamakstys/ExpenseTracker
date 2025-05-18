import { useState, useEffect } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Typography,
  List,
  ListItem,
  ListItemText,
  CircularProgress,
} from "@mui/material";
import type { SettleDTO } from "../types/ViewGroupDTO";

interface ViewSettlesModalProps {
  open: boolean;
  onClose: () => void;
  groupId: string;
  onSuccess: () => void;
}

export default function ViewSettlesModal({
    open,
    onClose,
    groupId,
    onSuccess
 }: ViewSettlesModalProps) {
  const [settles, setSettles] = useState<SettleDTO[]>([]);
  const [loading, setLoading] = useState(false);
  const [settlingId, setSettlingId] = useState<number | null>(null);
  const apiUrl = import.meta.env.VITE_API_URL;
  const userId = localStorage.getItem("id");

  useEffect(() => {
    if (open) {
      fetchSettles();
    }
  }, [open]);

  const fetchSettles = async () => {
    setLoading(true);
    try {
      const response = await fetch(`${apiUrl}/api/transaction/get-splits`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ UserId: userId, GroupId: groupId }),
      });
      if (response.ok) {
        const data = await response.json();
        setSettles(data);
      } else {
        console.error("Failed to fetch settles");
      }
    } catch (err) {
      console.error("Error fetching settles:", err);
    } finally {
      setLoading(false);
    }
  };

  const handleSettle = async (splitId: number) => {
    setSettlingId(splitId);
    try {
      const response = await fetch(`${apiUrl}/api/transaction/settle`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ SplitId: splitId, UserId: userId }),
      });
      if (response.ok) {
        await fetchSettles();
        onSuccess();
      } else {
        const data = await response.json();
        if (data.error) {
          console.error(data.error);
        }
      }
    } catch (err) {
      console.error("Error settling transaction:", err);
    } finally {
      setSettlingId(null);
    }
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>Settle Transactions</DialogTitle>
      <DialogContent>
        {loading ? (
          <CircularProgress />
        ) : settles.length === 0 ? (
          <Typography>No transactions to settle.</Typography>
        ) : (
          <List>
            {settles.map((settle) => (
              <ListItem
                key={settle.splitId}
                secondaryAction={
                  <Button
                    variant="contained"
                    color="primary"
                    size="small"
                    onClick={() => handleSettle(settle.splitId)}
                    disabled={settlingId === settle.splitId}
                  >
                    {settlingId === settle.splitId ? "Settling..." : "Settle"}
                  </Button>
                }
              >
                <ListItemText
                  primary={settle.userName}
                  secondary={`Amount: €${settle.amount.toFixed(2)}`}
                />
              </ListItem>
            ))}
          </List>
        )}
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Close</Button>
      </DialogActions>
    </Dialog>
  );
}
