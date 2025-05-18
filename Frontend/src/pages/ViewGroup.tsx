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

export default function ViewGroup() {
  const navigate = useNavigate();
  const [group, setGroup] = useState<ViewGroupDTO | null>(null);
  const apiUrl = import.meta.env.VITE_API_URL;
  const [addMemberOpen, setAddMemberOpen] = useState(false);
  const { groupId } = useParams();

  useEffect(() => {
    if (groupId) {
      fetchGroup(groupId);
    }
  }, [groupId]);

  const fetchGroup = async (groupId: string) => {
    const userId = localStorage.getItem("id");
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
          <Button variant="outlined" onClick={() => navigate(-1)}>
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
            <Button variant="outlined" onClick={() => setAddMemberOpen(true)}>
              Add Member
            </Button>
          </Stack>
          <Divider sx={{ mb: 2 }} />
          {group?.members?.length ? (
            <List disablePadding>
              {group.members.map((member) => (
                <Stack
                  direction="row"
                  justifyContent="space-between"
                  alignItems="center"
                  mb={3}
                >
                  <ListItem key={member.id} sx={{ px: 0, py: 1 }}>
                    <ListItemText
                      primary={member.name}
                      secondary={`Balance: €${member.balance.toFixed(2)}`}
                      primaryTypographyProps={{ fontWeight: 500 }}
                    />
                  </ListItem>
                  <Button
                    variant="outlined"
                    color="error"
                    onClick={() => removeMember(member.id)}
                  >
                    Remove
                  </Button>
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

        {/* Transactions Section */}
        <Box>
          <Typography variant="h6" fontWeight={600} gutterBottom>
            Transactions
          </Typography>
          <Divider sx={{ mb: 2 }} />
          {group?.transactions?.length ? (
            <Stack spacing={2}>
              {group.transactions.map((tx) => (
                <Paper
                  key={tx.id}
                  elevation={2}
                  sx={{ p: 2, backgroundColor: "#fefefe" }}
                >
                  <Typography fontWeight={600}>{tx.description}</Typography>
                  <Typography color="text.secondary" variant="body2">
                    Amount: €{tx.amount.toFixed(2)} | By: {tx.createdBy}
                  </Typography>
                </Paper>
              ))}
            </Stack>
          ) : (
            <Typography color="text.secondary">
              No transactions recorded.
            </Typography>
          )}
        </Box>
      </Paper>
    </Box>
  );
}
