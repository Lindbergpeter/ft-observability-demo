#### **Afleveringspakke – FT.Observability**

Dette er en guide til en quick-start, hvis løsningen ønskes at blive prøvekørt.

Denne pakke indeholder kildekode til FT.Observability samt en anonymiseret DemoService, der kan anvendes til at afprøve bibliotekets integration med OpenTelemetry, Elasticsearch og Kibana.



De konkrete services fra Folketingets IT, herunder PdfProvider og InfoFileExporter, er ikke vedlagt af hensyn til interne systemer. DemoService er derfor udviklet som en minimal service, der demonstrerer samme integrationsmønster.



#### **Indhold:**



\- **FT.Observability** / Shared library til standardisering af logging og distributed tracing i .NET-services.

\- **DemoService** / Minimal anonym demo-service, der anvender FT.Observability via project reference. Der er derfor ikke behov for installation af NuGet-pakke for at afprøve løsningen.

\- **docker-compose.yml** / Starter Elasticsearch, Kibana og OpenTelemetry Collector.

\- **otel-collector-config.yaml** / Konfiguration af Collector pipelines til logs og traces.




#### **Forudsætninger**



- .NET SDK
- Docker desktop (nok også login)
- Virtualisering aktiveret på maskinen
- WSL/Linux installeret til Docker Desktop

Bemærk, første opstart kan tage lidt tid, da Docker skal hente nødvendige images.




#### **Start observability-stack**



Kør i terminal fra roden af afleveringsmappen:


docker compose up


Dette starter:


Elasticsearch på http://localhost:9200

Kibana på http://localhost:5601

OpenTelemetry Collector på localhost:4317



#### **Start DemoService**


I en ny terminal skriv: 
cd DemoService
dotnet run --project DemoService

(Selvom der står "building..." kan servicen være startet korrekt. Gå eventuelt videre og test endpoint i browser)


DemoService kører som udgangspunkt på:

http://localhost:5094



#### **Test endpoint**



Kald endpointet i din browser:



http://localhost:5094/demo


Endpointet opretter et span og skriver en logbesked.



#### **Verificering i Kibana**



**Åbn Kibana:**



http://localhost:5601



I sidepanel, vælg Discover 
Hvis data views ikke findes, skal der oprettes Data Views.
Tryk Create Data view:

Data view 1 
Name:
ft-observability-traces

Index pattern:
ft-observability-traces

Timestamp field:
@timestamp


Data view 2:
Name:
ft-observability-logs

Index pattern:
ft-observability-logs

Timestamp field:
@timestamp


Der bør så bære være:
- Et trace med navnet demo.request
- en log med teksten Demo endpoint called
- Samme TraceId og SpanId på både log og trace



Bemærkning

DemoService anvender en project reference til FT.Observability for at gøre afleveringspakken direkte kørbar. I den reelle løsning er biblioteket tænkt distribueret som intern NuGet-pakke.

PdfProvider og InfoFileExporter er ikke vedlagt, da disse er eksisterende interne services fra Folketingets IT. DemoService demonstrerer samme integrationsmønster i en anonymiseret og selvstændigt kørbar løsning.