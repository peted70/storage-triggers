# Storage Triggers

Each sample provides a mechanism to respond to a file being uploaded to Azure Blob Storage. The reponse will be to place a message onto a Azure Service bus instance and thus trigger a workflow of some kind:

- Blob Trigger

- Poll Blob

- Event Grid

- Send Message