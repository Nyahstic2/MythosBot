
# MythosBot
Um bot do Discord para organizar personagens, missões e itens do seu RPG de uma forma centralizada.

## Índice
- [Descrição](#descrição)
- [Funcionalidades](#funcionalidades)
- [Instalação](#instalação)
- [Uso](#uso)
- [Roadmap](#roadmap)
- [Contribuição](#contribuição)
- [Licença](#licença)

## Descrição
O MythosBot foi desenvolvido para facilitar a organização de mesas de RPG, permitindo que mestres e jogadores gerenciem personagens, missões, itens e NPCs diretamente pelo Discord. Ideal para parties pequenas.

## Funcionalidades
- [ ] Criar e deletar personagens.
- [ ] Gerenciar inventário com criação de itens.
- [ ] Criar e gerenciar missões.
- [ ] Criar NPCs, incluindo templates pré-definidos.
- [ ] Comunicação integrada via slash-commands do Discord.

## Instalação
1. Certifique-se de ter as seguintes dependências instaladas:
   - **.NET Framework SDK 9.0**
   - **Discord.NET**
2. Clone este repositório:
   ```bash
   git clone https://github.com/Nyahstic2/MythosBot.git
   ```
3. Instale as dependências:
   ```bash
   dotnet restore
   ```
4. Configure o token do bot no arquivo `config.json`[^1]:
   ```json
   {
     "DISCORD_BOT_TOKEN" : "seu_token_aqui"
   }
   ```
5. Execute o bot:
   ```bash
   dotnet run
   ```

## Uso
Trabalho em progresso...

## Roadmap
- [ ] Implementação básica do bot (echo, ping)
- [ ] Criar e deletar personagens
- [ ] Criação de itens
- [ ] Criação de missões
- [ ] Criação de NPCs
- [ ] Criação de NPCs com base em um template

## Contribuição
Contribuições são bem-vindas! Siga os passos abaixo:
1. Faça um fork do projeto.
2. Crie uma branch para sua funcionalidade/ajuste:
   ```bash
   git checkout -b minha-melhoria
   ```
3. Faça um commit com suas mudanças:
   ```bash
   git commit -m "Adicionei uma nova funcionalidade"
   ```
4. Envie suas mudanças:
   ```bash
   git push origin minha-melhoria
   ```
5. Abra um Pull Request no repositório principal.

## Licença
Este projeto está licenciado sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

[^1]: Versões futuras do bot usará .env variables em vez de config.json
