FROM denoland/deno AS installer

EXPOSE 6969

WORKDIR /app

ADD / /app

RUN deno install
RUN deno task build


FROM denoland/deno

WORKDIR /app

COPY --from=installer /app/build ./build

CMD ["task", "prod"]
