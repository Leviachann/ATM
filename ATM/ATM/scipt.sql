CREATE TABLE Users
(
    CardNumber NVARCHAR(50),
    UserName NVARCHAR(50),
    Balance FLOAT
);

INSERT INTO Users (CardNumber, UserName, Balance)
VALUES (N'123456789', N'John Doe', 5000.0),
       (N'987654321', N'Jane Smith', 3500.0);
