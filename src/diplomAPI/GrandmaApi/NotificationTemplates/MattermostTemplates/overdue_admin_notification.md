:older_woman: Вас беспокоит АИС "БАБУЛЯ" :older_woman:

{{ for entity in Entities }}
Пользователь {{ entity.Name }} не вернул устройства:

| Устройство | Номер | Дата получения | Дата возврата |
|-----|-------|----------------|---------------|
{{for device in entity.Devices}}|{{device.Name}}|{{device.InventoryNumber}}|{{device.TakeAt}}|{{device.ReturnAt}}|
{{ end }}
{{ end }}
БАБУЛЯ негодует и ты негодуй!

Данное письмо сформировано автоматически и не требует ответа