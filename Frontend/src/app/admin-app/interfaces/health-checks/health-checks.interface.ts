export interface ServiceHealth {
  name: string;
  response: HealthCheckResponse | null;
  error: string | null;
}

export interface HealthCheckResponse {
  status: string;
  totalDuration: string;
  entries: { [key: string]: HealthCheckEntry };
}

export interface HealthCheckEntry {
  data: any;
  duration: string;
  status: string;
  tags: string[];
}