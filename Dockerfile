ARG VERSION=8u151

FROM openjdk:${VERSION}-jdk as BUILD

COPY . /src
WORKDIR /src
RUN ./gradlew --no-daemon installDist

FROM openjdk:${VERSION}-jre

COPY ./.env /build/
COPY --from=BUILD /src/build/install/OurGuardianBot/ /build/

WORKDIR /build/

ENTRYPOINT ./bin/OurGuardianBot
