CREATE DATABASE "IdentityServiceDb";
CREATE DATABASE "ProjectsServiceHangfireDb";

\c IdentityServiceDb
GRANT ALL PRIVILEGES ON DATABASE "IdentityServiceDb" TO postgres;

\c ProjectsServiceHangfireDb
GRANT ALL PRIVILEGES ON DATABASE "ProjectsServiceHangfireDb" TO postgres;