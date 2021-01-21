USE master;
GO

IF DB_ID (N'SMDRLog') IS NULL
CREATE DATABASE SMDRLog;
GO

USE SMDRLog;
GO

Create Table CallLog
(
    Record_GUID UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    Record_CreationDate DATETIME NOT NULL DEFAULT GETDATE(),
    Record_ClientIP [VARCHAR](45) NOT NULL,
    [Call Start] DATETIME NOT NULL,
    [Connected Time] [TIME](7) NOT NULL,
    [Ring Time] INT NULL,
    [Caller] [VARCHAR](255) NOT NULL,
    [Direction] [VARCHAR](1) NOT NULL,
    [Called Number] [VARCHAR](255) NOT NULL,
    [Dialled Number] [VARCHAR](255) NOT NULL,
    [Account] [VARCHAR](255) NULL,
    [Is Internal] BIT NOT NULL DEFAULT 0,
    [Call ID] INT NOT NULL,
    [Continuation] BIT NOT NULL DEFAULT 0,
    [Party1Device] [VARCHAR](255) NULL,
    [Party1Name] [VARCHAR](255) NULL,
    [Party2Device] [VARCHAR](255) NULL,
    [Party2Name] [VARCHAR](255) NULL,
    [Hold Time] INT NULL,
    [Park Time] INT NULL,
    [AuthValid] BIT NULL,
    [AuthCode] [VARCHAR](5) NULL,
    [User Charged] [VARCHAR](255) NULL,
    [Call Charge] [VARCHAR](255) NULL,
    [Currency] [VARCHAR](255) NULL,
    [Amount at Last User Change] [VARCHAR](255) NULL,
    [Call Units] INT NULL,
    [Units at Last User Change] INT NULL,
    [Cost per Unit] INT NULL,
    [Mark Up] INT NULL,
    [External Targeting Cause] [VARCHAR](255) NULL,
    [External Targeter Id] [VARCHAR](255) NULL,
    [External Targeted Number] [VARCHAR](255) NULL,
    [Server IP Address of the Caller Extension] [VARCHAR](45) NULL,
    [Unique Call ID for the Caller Extension] INT NULL,
    [Server IP Address of the Called Extension] [VARCHAR](45) NULL,
    [Unique Call ID for the Called Extension] INT NULL,
    [UTC Time] DATETIME NULL,
    CONSTRAINT PK_CallLog PRIMARY KEY (Record_GUID, Record_CreationDate)
);
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'Auto-generated record ID (GUID) - PK.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Record_GUID';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'Auto-generated record creation timestamp - PK.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Record_CreationDate';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'IP Address of SMDR client device that reported this record.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Record_ClientIP';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'Call start time. For all transferred call segment this is the time the call was initiated, so each segment of the call has the same call start time.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Call Start';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'Duration of the connected part of the call in HH:MM:SS format. This does not include ringing, held and parked time. A lost or failed call will have a duration of 00:00:00. The total duration of a record is calculated as Connected Time + Ring Time + Hold Time + Park Time.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Connected Time';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'Duration of the ring part of the call in seconds. For inbound calls this represents the interval between the call arriving at the switch and it being answered, not the time it rang at an individual extension. For outbound calls, this indicates the interval between the call being initiated and being answered at the remote end if supported by the trunk type. Analog trunks are not able to detect remote answer and therefore cannot provide a ring duration for outbound calls.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Ring Time';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'The callers'' number. If the call was originated at an extension, this will be that extension number. If the call originated externally, this will be the CLI of the caller if available, otherwise blank. For SIP trunks, the field can contain the number plus IP address. For example 12345@192.0.2.123.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Caller';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'Direction of the call ; I for Inbound, O for outbound. Internal calls will be represented as outbound and should be cross-referenced with their ''Is Internal'' value to determine if the call is internal, external outbound or external inbound.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Direction';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'This is the number called by the system. For a call that is transferred, this field shows the original called number, not the number of the party who transferred the call.  Internal calls: The extension, group or short code called. Inbound calls: The target extension number for the call. Outbound calls: The dialed digits. Voice Mail: Calls to a user''s own voicemail mailbox'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Called Number';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'For internal calls and outbound calls, this is identical to the Called Number above. For inbound calls, this is the DDI of the incoming caller.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Dialled Number';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'The last account code attached to the call.  Note: System account codes may contain alphanumeric characters.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Account';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'0 or 1, denoting whether both parties on the call are internal or external (1 being an internal call). Calls to destinations on other switches in a network are indicated as internal.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Is Internal';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'This is a numerical identifier, starting at 1,000,000 which is incremented for each unique call. If the call has generates several SMDR records, each record will have the same Call ID. Note that the Call ID used is restarted from 1,000,000 if the system is restarted.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Call ID';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'1 if there is a further record for this call id, 0 otherwise.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Continuation';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'The device 1 number. This is usually the call initiator though in some scenarios such as conferences this may vary. If an extension/hunt group is involved in the call its details will have priority over a trunk. That includes remote network destinations.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Party1Device';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'The name of the device. For an extension or agent, this is the user name encoded in UTF-8.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Party1Name';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'The other party for the SMDR record of this call segment. Encoded as per Party1Device above. For barred calls, this field is populated with "Barred".'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Party2Device';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'The other party for the SMDR record of this call segment. See Party1Name. For barred calls, this field is populated with "Barred".'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Party2Name';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'The amount of time in seconds the call has been held during this call segment.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Hold Time';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'The amount of time in seconds the call has been parked during this call segment.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Park Time';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'This field is used for authorization codes. This field shows 1 for valid authorization or 0 for invalid authorization.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'AuthValid';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'For security, this field shows n/a regardless of whether an authorization code was used.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'AuthCode';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'This and the following fields are used for ISDN Advice of Charge (AoC). The user to which the call charge has been assigned. This is not necessarily the user involved in the call.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'User Charged';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'The total call charge calculated using the line cost per unit and user markup.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Call Charge';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'The currency. This is a system wide setting set in the system configuration.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Currency';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'The current AoC amount at user change.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Amount at Last User Change';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'The total call units.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Call Units';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'The current AoC units at user change.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Units at Last User Change';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'This value is set in the system configuration against each line on which Advice of Charge signalling is set. The values are 1/10,000th of a currency unit. For example if the call cost per unit is £1.07, a value of 10700 should be set on the line.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Cost per Unit';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'Indicates the mark up value set in the system configuration for the user to which the call is being charged. The field is in units of 1/100th, for example an entry of 100 is a markup factor of 1.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Mark Up';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'This field indicates who or what caused the external call and a reason code. For example UFU indicates that the external call was caused by
the Forward Unconditional setting of a User.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'External Targeting Cause';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'The associated name of the targeter indicated in the External Targeting Cause field. For hunt groups and users this will be their name in the system configuration. For an Incoming Call Route this will be the Tag if set, otherwise ICR.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'External Targeter ID';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'This field is used for forwarded, Incoming Call Route targeted and mobile twin calls to an external line. It shows the external number called by the system as a result of the off switch targeting where as other called fields give the original number dialled.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'External Targeted Number';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'This IP address identifies the server where the call was initiated. If the field does not contain an IP address, then the call originated outside the IP Office network.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Server IP Address of the Caller Extension';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'Numerical value that is a unique identifier of the call on the server where the call was initiated.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Unique Call ID for the Caller Extension';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'This IP address identifies the server where the called extension is logged in. If the field does not contain an IP address, then the call is to a trunk outside the IP Office network.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Server IP Address of the Called Extension';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'Numerical value that is a unique identifier of the call on the server where the called extension is logged in.'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'Unique Call ID for the Called Extension';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description'
   , @value=N'Coordinated Universal Time (UTC)'
   , @level0type=N'SCHEMA'
   , @level0name=N'dbo'
   , @level1type=N'TABLE'
   , @level1name=N'CallLog'
   , @level2type=N'COLUMN'
   , @level2name=N'UTC Time';
GO
