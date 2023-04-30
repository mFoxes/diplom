:older_woman: Вас беспокоит АИС "БАБУЛЯ" :older_woman:

Бабуля негодует, потому что вы не вернули вовремя устройства:

| Устройство | Номер | Дата получения | Дата возврата |
|-----|-------|----------------|---------------|
{{ for device in Entities }}|{{ device.Name }}|{{ device.InventoryNumber }}|{{ device.TakeAt }}|{{ device.ReturnAt }}|
{{ end }}

Впредь будьте внимательны в темных переулках...

Данное письмо сформировано автоматически и не требует ответа. Но ответить вам придется.

![Granny](https://thumbs.gfycat.com/ClosePortlyHoneybadger-size_restricted.gif)