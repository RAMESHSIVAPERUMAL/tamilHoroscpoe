-- Migration: Add LastDailyFeeDeductionDate to Users table
-- Date: 2024
-- Description: Add column to track last daily fee deduction date for horoscope generation

-- Add the column
ALTER TABLE Users 
ADD LastDailyFeeDeductionDate DATETIME NULL;

-- Add index for better query performance
CREATE INDEX IX_Users_LastDailyFeeDeductionDate 
ON Users(LastDailyFeeDeductionDate);

-- Optional: Add comment to the column
EXEC sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Last date when daily horoscope fee was deducted. Used to track if today''s fee has been paid.', 
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'Users',
    @level2type = N'COLUMN', @level2name = N'LastDailyFeeDeductionDate';
