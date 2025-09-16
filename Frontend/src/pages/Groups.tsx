import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Button,
  Box,
  Typography,
  Paper,
  Stack,
  List,
  ListItem,
  ListItemText,
  Divider,
} from "@mui/material";
import CreateGroupModal from "../components/CreateGroupModal";

export default function Groups() {
  const [groups, setGroups] = useState<any[]>([]);
  const [open, setOpen] = useState(false);
  const apiUrl = import.meta.env.VITE_API_URL;
  const navigate = useNavigate();

  useEffect(() => {
    fetchGroups();
  }, []);

  const fetchGroups = async () => {
    const id = localStorage.getItem("id");
    const response = await fetch(`${apiUrl}/group/getByUser/${id}`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
    });

    if (response.ok) {
      const data = await response.json();
      setGroups(data.groups);
    } else {
      console.error("Failed to fetch groups");
    }
  };

  return (
    <Box
      sx={{
        minHeight: "100vh",
        bgcolor: "#f9f9f9",
        display: "flex",
        justifyContent: "center",
        py: 6,
        px: 2,
      }}
    >
      <Paper
        elevation={4}
        sx={{
          width: "100%",
          maxWidth: 500,
          p: 3,
          borderRadius: 3,
        }}
      >
        <Stack
          direction="row"
          justifyContent="space-between"
          alignItems="center"
          mb={3}
        >
          <Typography variant="h5" fontWeight={600}>
            Your Groups
          </Typography>
          <Button variant="contained" onClick={() => setOpen(true)}>
            Create
          </Button>
        </Stack>

        <Divider sx={{ mb: 3 }} />

        <CreateGroupModal
          open={open}
          onClose={() => setOpen(false)}
          onSuccess={fetchGroups}
        />

        {groups.length === 0 ? (
          <Typography
            color="text.secondary"
            align="center"
            sx={{ mt: 4, fontStyle: "italic" }}
          >
            No groups found. Start by creating your first group.
          </Typography>
        ) : (
          <List>
            {groups.map((group) => (
              <Paper
                key={group.id}
                sx={{
                  mb: 2,
                  borderRadius: 2,
                  boxShadow: 1,
                  transition: "transform 0.2s ease",
                  "&:hover": {
                    transform: "scale(1.02)",
                  },
                }}
                onClick={() => {
                  navigate(`/groups/${group.id}`);
                }}
              >
                <ListItem>
                  <ListItemText
                    primary={
                      <Typography variant="subtitle1" fontWeight={600}>
                        {group.title}
                      </Typography>
                    }
                    secondary={
                      <Typography color="text.secondary">
                        Balance: €{group.balance}
                      </Typography>
                    }
                  />
                </ListItem>
              </Paper>
            ))}
          </List>
        )}
      </Paper>
    </Box>
  );
}
