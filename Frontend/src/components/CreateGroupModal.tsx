import React, { useState } from "react";
import {
  Button,
  Modal,
  Box,
  Typography,
  TextField,
  Stack,
} from "@mui/material";

interface CreateGroupModalProps {
  open: boolean;
  onClose: () => void;
  onSuccess: () => void;
}

export default function CreateGroupModal({
  open,
  onClose,
  onSuccess,
}: CreateGroupModalProps) {
  const [groupName, setGroupName] = useState("");
  const [error, setError] = useState("");

  const apiUrl = import.meta.env.VITE_API_URL;

  const handleCreateGroup = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    const id = localStorage.getItem("id");
    const response = await fetch(`${apiUrl}/api/group/create`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
      body: JSON.stringify({ GroupName: groupName, UserId: id }),
    });
    if (response.ok) {
      setGroupName("");
      setError("");
      onClose();
      onSuccess();
    } else {
      setError("Failed to create group.");
    }
  };

  return (
    <Modal open={open} onClose={onClose}>
      <Box
        sx={{
          position: "absolute",
          top: "50%",
          left: "50%",
          transform: "translate(-50%, -50%)",
          bgcolor: "background.paper",
          boxShadow: 24,
          p: 4,
          minWidth: 300,
          borderRadius: 2,
        }}
      >
        <form onSubmit={handleCreateGroup}>
          <Stack spacing={2}>
            <Typography variant="h6">Create Group</Typography>
            <TextField
              label="Name"
              value={groupName}
              onChange={(e) => setGroupName(e.target.value)}
              fullWidth
              required
              autoFocus
            />
            {error && (
              <Typography color="error" variant="body2">
                {error}
              </Typography>
            )}
            <Stack direction="row" spacing={2} justifyContent="flex-end">
              <Button onClick={onClose} color="secondary">
                Cancel
              </Button>
              <Button type="submit" variant="contained">
                Create
              </Button>
            </Stack>
          </Stack>
        </form>
      </Box>
    </Modal>
  );
}
