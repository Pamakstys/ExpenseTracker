import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Box,
  Button,
  TextField,
  Typography,
  Paper,
  Stack,
} from "@mui/material";

export default function Login() {
  const [isLogin, setIsLogin] = useState(true);
  const [name, setName] = useState("");
  const [error, setError] = useState("");
  const navigate = useNavigate();

  const apiUrl = import.meta.env.VITE_API_URL;

  const handleLogin = (
    endpoint: string,
    e: React.FormEvent<HTMLFormElement>
  ) => {
    e.preventDefault();
    setError("");

    fetch(`${apiUrl}/user/${endpoint}`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ name }),
    })
      .then((response) => {
        response.json().then((data) => {
          if (response.ok) {
            localStorage.setItem("id", data.id);
            localStorage.setItem("name", data.name);
            navigate("/");
          } else {
            if (isLogin) {
                setError("Login failed: " + data.error);
            } else {
              setError("Registration failed: " + data.error);
            }
          }
        });
      })
      .catch(() => {
        setError("An error occurred. Please try again.");
      });
  };

  return (
    <Box
      sx={{
        minHeight: "100vh",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        bgcolor: "#f5f5f5",
      }}
    >
      <Paper elevation={3} sx={{ p: 4, minWidth: 320 }}>
        <Stack spacing={3} alignItems="center">
          <Typography variant="h5" fontWeight={700}>
            {isLogin ? "Login" : "Register"}
          </Typography>
          <Box
            component="form"
            onSubmit={(e) => handleLogin(isLogin ? "login" : "register", e)}
            width="100%"
          >
            <TextField
              label="Name"
              variant="outlined"
              fullWidth
              required
              value={name}
              onChange={(e) => setName(e.target.value)}
              autoFocus
            />
            <Button
              type="submit"
              variant="contained"
              color="primary"
              fullWidth
              sx={{ mt: 2 }}
            >
              {isLogin ? "Login" : "Register"}
            </Button>
          </Box>
          <Button
            variant="text"
            onClick={() => {
              setIsLogin(!isLogin);
              setError("");
            }}
            sx={{ mt: 1 }}
          >
            {isLogin ? "Go to Register" : "Go to Login"}
          </Button>
          {error && (
            <Typography
              color="error"
              sx={{ width: "100%", textAlign: "center" }}
            >
              {error}
            </Typography>
          )}
        </Stack>
      </Paper>
    </Box>
  );
}
