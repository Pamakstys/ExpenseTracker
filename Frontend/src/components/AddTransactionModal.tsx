import { useState } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Stepper,
  Step,
  StepLabel,
  Button,
  Typography,
} from "@mui/material";
import type { GroupMemberDTO } from "../types/ViewGroupDTO";

interface SplitDTO {
  userId: number;
  amount: number;
}

interface AddTransactionModalProps {
  open: boolean;
  onClose: () => void;
  groupId: string;
  onSuccess: () => void;
  members: GroupMemberDTO[];
}

export default function AddTransactionModal({
  open,
  onClose,
  groupId,
  onSuccess,
  members,
}: AddTransactionModalProps) {
  const [step, setStep] = useState(0);
  const [amount, setAmount] = useState("");
  const [description, setDescription] = useState("");
  const [error, setError] = useState("");
  const [splitType, setSplitType] = useState<
    "Equally" | "Dynamic" | "Percentage"
  >("Equally");
  const [splits, setSplits] = useState<SplitDTO[]>([]);

  const apiUrl = import.meta.env.VITE_API_URL;
  const userId = localStorage.getItem("id");
  const steps = ["Amount", "Description", "Split Type", "Splits"];

  const handleNext = () => setStep((prev) => prev + 1);
  const handleBack = () => setStep((prev) => prev - 1);

  const handleSubmit = async () => {
    try {
      const response = await fetch(`${apiUrl}/transaction/create`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          amount: parseFloat(amount),
          description: description,
          userId: userId,
          groupId: parseInt(groupId),
          splitType: splitType,
          splits: splits,
        }),
      });

      if (response.ok) {
        onSuccess();
        handleReset();
      } else {
        // console.error("Failed to add transaction");
        const data = await response.json();
        if (data.error) {
          setError(data.error);
        }
      }
    } catch (err) {
      console.error("Error adding transaction:", err);
    }
  };

  const handleReset = () => {
    onClose();
    setStep(0);
    setAmount("");
    setDescription("");
    setSplitType("Equally");
    setSplits([]);
  };

  const renderStepContent = (stepIndex: number) => {
    switch (stepIndex) {
      case 0:
        return (
          <TextField
            fullWidth
            label="Amount"
            type="number"
            value={amount}
            onChange={(e) => setAmount(e.target.value)}
          />
        );
      case 1:
        return (
          <TextField
            fullWidth
            label="Description"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
        );
      case 2:
        return (
          <div style={{ display: "flex", gap: 12 }}>
            <Button
              variant={splitType === "Equally" ? "contained" : "outlined"}
              onClick={() => setSplitType("Equally")}
            >
              Equal
            </Button>
            <Button
              variant={splitType === "Dynamic" ? "contained" : "outlined"}
              onClick={() => setSplitType("Dynamic")}
            >
              Dynamic
            </Button>
            <Button
              variant={splitType === "Percentage" ? "contained" : "outlined"}
              onClick={() => setSplitType("Percentage")}
            >
              Percentage
            </Button>
          </div>
        );

      case 3:
        if (splitType === "Equally") {
          return <p>Members will be split equally.</p>;
        }

        return (
          <>
            {members.map((member) => {
              const value =
                splits.find((s) => s.userId === member.id)?.amount ?? 0;

              return (
                <div
                  key={member.id}
                  style={{
                    display: "flex",
                    alignItems: "center",
                    gap: 8,
                    marginBottom: 8,
                  }}
                >
                  <span style={{ minWidth: 120 }}>{member.name}</span>
                  <TextField
                    label={splitType === "Percentage" ? "Percentage" : "Amount"}
                    type="number"
                    InputProps={{
                      endAdornment:
                        splitType === "Percentage" ? "%" : undefined,
                    }}
                    value={value}
                    onChange={(e) => {
                      const inputVal = parseFloat(e.target.value);
                      setSplits((prev) => {
                        const updated = [...prev];
                        const index = updated.findIndex(
                          (s) => s.userId === member.id
                        );
                        if (index !== -1) {
                          updated[index].amount = inputVal;
                        } else {
                          updated.push({ userId: member.id, amount: inputVal });
                        }
                        return updated;
                      });
                    }}
                  />
                </div>
              );
            })}
          </>
        );

      case 4:
        return (
          <div>
            <p>
              <strong>Amount:</strong> {amount}
            </p>
            <p>
              <strong>Description:</strong> {description}
            </p>
            <p>
              <strong>Split Type:</strong> {splitType}
            </p>
            <p>
              <strong>Splits:</strong>
            </p>
            <ul>
              {splits.map((s, i) => (
                <li key={i}>
                  User ID: {s.userId}, Amount: {s.amount}
                </li>
              ))}
            </ul>
          </div>
        );
      default:
        return null;
    }
  };

  return (
    <Dialog open={open} onClose={handleReset} maxWidth="sm" fullWidth>
      <DialogTitle>Add Transaction</DialogTitle>
      <DialogContent>
        <Stepper activeStep={step} alternativeLabel>
          {steps.map((label) => (
            <Step key={label}>
              <StepLabel>{label}</StepLabel>
            </Step>
          ))}
        </Stepper>

        <div style={{ marginTop: 20 }}>{renderStepContent(step)}</div>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleReset}>Cancel</Button>
        {step > 0 && <Button onClick={handleBack}>Back</Button>}
        {step < steps.length - 1 ? (
          <Button onClick={handleNext}>Next</Button>
        ) : (
          <Button onClick={handleSubmit} variant="contained" color="primary">
            Submit
          </Button>
        )}
      </DialogActions>
      {error && (
        <Typography
          variant="body2"
          color="error"
          style={{ marginTop: 16, textAlign: "center", marginBottom: 16, fontSize:  16}}
        >
          {error}
        </Typography>
      )}
    </Dialog>
  );
}
