FROM node:22-alpine
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci
COPY . .

EXPOSE 4200
CMD ["npm", "run", "start"]