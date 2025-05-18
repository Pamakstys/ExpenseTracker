import {
  Box,
  Typography,
  Paper,
  Stack,
  Button,
  List,
  ListItem,
  ListItemText,
  Divider,
} from "@mui/material";
import { useNavigate, useParams } from "react-router-dom";
import { useState, useEffect } from "react";
import type { ViewGroupDTO } from "../types/ViewGroupDTO";
import AddMemberModal from "../components/AddMemberModal";
import AddTransactionModal from "../components/AddTransactionModal";
import ViewSettlesModal from "../components/ViewSettlesModal";
import ViewTransactionModal from "../components/ViewTransactionModal";

export default function ViewGroup() {
  const navigate = useNavigate();
  const [group, setGroup] = useState<ViewGroupDTO | null>(null);
  const apiUrl = import.meta.env.VITE_API_URL;
  const [addMemberOpen, setAddMemberOpen] = useState(false);
  const [addTransactionOpen, setAddTransactionOpen] = useState(false);
  const [settleOpen, setSettleOpen] = useState(false);
  const [viewTransactionOpen, setViewTransactionOpen] = useState(false);
  const [selectedTransaction, setSelectedTransaction] = useState<number | null>(null);
  const { groupId } = useParams();
  const userId = localStorage.getItem("id");
  useEffect(() => {
    if (groupId) {
      fetchGroup(groupId);
    }
  }, [groupId]);

  const fetchGroup = async (groupId: string) => {
    const response = await fetch(`${apiUrl}/api/group/view`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        UserId: userId,
        GroupId: groupId,
      }),
    });
    if (response.ok) {
      const data = await response.json();
      setGroup(data);
    } else {
      navigate("/groups");
    }
  };

  const removeMember = async (userId: string | number) => {
    if (!groupId) return;
    try {
      const response = await fetch(`${apiUrl}/api/group/removeMember`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          UserId: userId,
          GroupId: groupId,
        }),
      });
      if (response.ok) {
        fetchGroup(groupId);
      } else {
        console.error("Failed to remove member");
      }
    } catch (err) {
      console.error("Error removing member:", err);
    }
  };

  return (
    <Box sx={{ minHeight: "100vh", bgcolor: "#f0f2f5", px: 2, py: 4 }}>
      <Paper
        elevation={4}
        sx={{
          maxWidth: 700,
          mx: "auto",
          p: 4,
          borderRadius: 4,
        }}
      >
        <Stack
          direction="row"
          justifyContent="space-between"
          alignItems="center"
          mb={3}
        >
          <Typography variant="h4" fontWeight={700}>
            {group?.title ?? "Group Details"}
          </Typography>
          <Button variant="contained" onClick={() => navigate(-1)}>
            Go Back
          </Button>
        </Stack>

        <Box mb={4}>
          <Stack
            direction="row"
            justifyContent="space-between"
            alignItems="center"
            mb={3}
          >
            <Typography variant="h6" fontWeight={600}>
              Members
            </Typography>
            <Button
              variant="outlined"
              color="success"
              onClick={() => setAddMemberOpen(true)}
            >
              Add Member
            </Button>
          </Stack>
          <Divider sx={{ mb: 2 }} />
          <div style={{ display: "flex", justifyContent: "end" }}>
            {group?.members &&
              (() => {
                const currentMember = group.members.find(
                  (m) => m.id === Number(userId)
                );
                return currentMember && currentMember.balance > 0;
              })() && (
                <Button
                  variant="outlined"
                  color="primary"
                  sx={{ ml: 1 }}
                  style={{ marginBottom: "16px" }}
                  onClick={() => setSettleOpen(true)}
                >
                  Settle
                </Button>
              )}
          </div>
          <Divider sx={{ mb: 2 }} />
          {group?.members?.length ? (
            <List disablePadding>
              {group.members.map((member) => (
                <Stack
                  key={member.id}
                  direction="row"
                  justifyContent="space-between"
                  alignItems="center"
                  mb={3}
                >
                  <ListItem key={member.id} sx={{ px: 0, py: 1 }}>
                    <ListItemText
                      primary={member.name}
                      secondary={`Balance owed: €${member.balance.toFixed(2)}`}
                    />
                  </ListItem>
                  {(member.id !== Number(userId) && member.balance === 0) && (
                    <>
                      <Button
                        variant="outlined"
                        color="error"
                        onClick={() => removeMember(member.id)}
                      >
                        Remove
                      </Button>
                    </>
                  )}
                </Stack>
              ))}
            </List>
          ) : (
            <Typography color="text.secondary">No members found.</Typography>
          )}
        </Box>

        {groupId && (
          <AddMemberModal
            open={addMemberOpen}
            onClose={() => setAddMemberOpen(false)}
            groupId={groupId}
            apiUrl={apiUrl}
            memberIds={group?.members?.map((m) => m.id) ?? []}
            onSuccess={() => {
              setAddMemberOpen(false);
              fetchGroup(groupId);
            }}
          />
        )}

        {groupId && (
          <AddTransactionModal
            open={addTransactionOpen}
            onClose={() => setAddTransactionOpen(false)}
            groupId={groupId}
            members={group?.members ?? []}
            onSuccess={() => {
              setAddTransactionOpen(false);
              fetchGroup(groupId);
            }}
          />
        )}

        {groupId && (
          <ViewTransactionModal
            open={viewTransactionOpen}
            onClose={() => {setViewTransactionOpen(false)}}
            groupId={groupId}
            transactionId={selectedTransaction}
            onSuccess={() => {
              // setSettleOpen(false);
              // fetchGroup(groupId);
            }}
          />
        )}

        {groupId && (
          <ViewSettlesModal
            open={settleOpen}
            onClose={() => {setSettleOpen(false), fetchGroup(groupId)}}
            groupId={groupId}
            onSuccess={() => {
              // setSettleOpen(false);
              // fetchGroup(groupId);
            }}
          />
        )}

        {group?.members && group.members.length > 1 && (
          <Box>
          <Stack
            direction="row"
            justifyContent="space-between"
            alignItems="center"
            mb={3}
          >
            <Typography variant="h6" fontWeight={600}>
              Transactions
            </Typography>
            <Button
              variant="outlined"
              color="success"
              onClick={() => setAddTransactionOpen(true)}
            >
              New Transaction
            </Button>
          </Stack>
          <Divider sx={{ mb: 2 }} />
          {group?.transactions?.length ? (
            <Stack spacing={2}>
              {group.transactions.map((tx) => (
                <Paper
                  key={tx.id}
                  elevation={2}
                  sx={{ p: 2, backgroundColor: "#fefefe" }}
                >
                  <Typography fontWeight={600}>{tx.userName}</Typography>
                  <Typography color="text.secondary" variant="body2">
                    Amount: €{tx.amount.toFixed(2)}
                  </Typography>
                  <Typography color="text.secondary" variant="body2">
                    Description: {tx.description}
                  </Typography>
                  <Button
                    variant="outlined"
                    color="error"
                    onClick={() => {setViewTransactionOpen(true), setSelectedTransaction(tx.id)}}
                    sx={{ mt: 1 }}
                  >
                    View
                  </Button>
                </Paper>
              ))}
            </Stack>
          ) : (
            <Typography color="text.secondary">
              No transactions recorded.
            </Typography>
          )}
        </Box>)}
      </Paper>
    </Box>
  );
}
