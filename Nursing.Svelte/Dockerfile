FROM node AS clone

WORKDIR /app

RUN apt-get update && apt-get install -y git
RUN git clone https://github.com/JLewinski/Nursing.git .
RUN git switch svelte-db

FROM node:current-alpine

EXPOSE 6969

WORKDIR /app

COPY --from=clone /app/Nursing.Svelte .

ENV DATABASE_URL=postgres://root:mysecretpassword@nursingdb:5432/local
ENV PORT=6969

RUN npm install
RUN npm run build
RUN rm -rf src/ static/ docker-compose.yml

USER node:node

CMD ["node", "build/index.js"]
