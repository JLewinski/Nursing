FROM node

EXPOSE 6969

WORKDIR /app

ADD / /app

ENV DATABASE_URL=postgres://root:mysecretpassword@localhost:5432/local
ENV PORT=6969

RUN npm ci
RUN npm run build
RUN rm -rf src/ static/ docker-compose.yml

USER node:node

CMD ["node", "build/index.js"]
