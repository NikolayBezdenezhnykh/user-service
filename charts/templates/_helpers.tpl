{{/*
Expand the name of the chart.
*/}}
{{- define "user-service.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}


{{/*
Create a default fully qualified app name.
*/}}
{{- define "user-service.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Return true if a secret object should be created
*/}}
{{- define "user-service.createSecret" -}}
{{- if not .Values.existingSecret }}
    {{- true -}}
{{- end }}
{{- end }}

{{/*
Get secretName.
*/}}
{{- define "user-service.secretName" -}}
{{- if .Values.existingSecret }}
    {{- printf "%s" (tpl .Values.existingSecret $) -}}
{{- else -}}
    {{- printf "%s" (include "user-service.fullname" .) -}}
{{- end -}}
{{- end -}}

{{/*
Return true if a configmap object should be created
*/}}
{{- define "user-service.createConfigMap" -}}
{{- if not .Values.existingConfigMap }}
    {{- true -}}
{{- end }}
{{- end }}

{{/*
Get configMap.
*/}}
{{- define "user-service.configmap" -}}
{{- if .Values.existingConfigMap }}
    {{- printf "%s" (tpl .Values.existingConfigMap $) -}}
{{- else -}}
    {{- printf "%s" (include "user-service.fullname" .) -}}
{{- end -}}
{{- end -}}