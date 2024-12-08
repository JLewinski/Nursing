FROM node

EXPOSE 6969

WORKDIR /app

ADD / /app

ENV DATABASE_URL=postgres://root:mysecretpassword@localhost:5432/local
ENV PORT=6969

RUN npm install
RUN npm run build

CMD ["npm", "run", "prod"]
