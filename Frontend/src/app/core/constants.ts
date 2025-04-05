
export const IDENTITY_SERVICE_API_URL: string = "http://localhost:5000/identity-service/api/";
export const PROJECTS_SERVICE_API_URL: string = "http://localhost:5000/projects-service/api/";
export const PAYMENTS_SERVICE_API_URL: string = "http://localhost:5000/payments-service/api/";
export const CHAT_SERVICE_HUB_URL: string = "http://localhost:5000/chat-service/hubs/chat";

export const PROJECT_STATUSES = [
  { value: 'Published', label: 'Published' },
  { value: 'AcceptingApplications', label: 'Accepting Applications' },
  { value: 'WaitingForWorkStart', label: 'Waiting For Work Start' },
  { value: 'InProgress', label: 'In Progress' },
  { value: 'PendingForReview', label: 'Pending For Review' },
  { value: 'Completed', label: 'Completed' },
  { value: 'Expired', label: 'Expired' },
  { value: 'Cancelled', label: 'Cancelled' }
];