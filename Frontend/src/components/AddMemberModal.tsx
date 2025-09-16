import React, { useEffect, useState } from "react";
import {
  Modal,
  Box,
  Typography,
  Button,
  Stack,
  MenuItem,
  Select,
  FormControl,
  InputLabel,
} from "@mui/material";

interface AddMemberModalProps {
  open: boolean;
  onClose: () => void;
  groupId: string;
  onSuccess: () => void;
  apiUrl: string;
  memberIds: (string | number)[];
}

export default function AddMemberModal({
  open,
  onClose,
  groupId,
  onSuccess,
  apiUrl,
  memberIds,
}: AddMemberModalProps) {
  const [users, setUsers] = useState<any[]>([]);
  const [selectedUser, setSelectedUser] = useState("");
  const [error, setError] = useState("");

  useEffect(() => {
    if (open) {
      fetch(`${apiUrl}/user/all`)
        .then((res) => res.json())
        .then((data) => {
          const currentUserId = localStorage.getItem("id");
          const filtered = data.filter(
            (user: any) =>
              String(user.id) !== String(currentUserId) &&
              !memberIds.map(String).includes(String(user.id))
          );
          setUsers(filtered);
        })
        .catch(() => setUsers([]));
    }
  }, [open, apiUrl, memberIds]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    if (!selectedUser) {
      setError("Please select a user.");
      return;
    }
    const response = await fetch(`${apiUrl}/group/addMember`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ GroupId: groupId, UserId: selectedUser }),
    });
    if (response.ok) {
      setSelectedUser("");
      onClose();
      onSuccess();
    } else {
      setError("Failed to add member.");
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
        <form onSubmit={handleSubmit}>
          <Stack spacing={2}>
            <Typography variant="h6">Add Member</Typography>
            <FormControl fullWidth required>
              <InputLabel id="user-select-label">User</InputLabel>
              <Select
                labelId="user-select-label"
                value={selectedUser}
                label="User"
                onChange={(e) => setSelectedUser(e.target.value)}
              >
                {users.map((user) => (
                  <MenuItem key={user.id} value={user.id}>
                    {user.name}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
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
                Add
              </Button>
            </Stack>
          </Stack>
        </form>
      </Box>
    </Modal>
  );
}
