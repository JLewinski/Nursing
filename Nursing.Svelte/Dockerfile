FROM denoland/deno AS installer

EXPOSE 6969

WORKDIR /app

ADD / /app

RUN deno install
RUN deno task build


CMD ["task", "prod"]
