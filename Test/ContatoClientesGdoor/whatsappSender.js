// ========================================
// MÓDULO DE ENVIO WHATSAPP
// ========================================

import DataManager from './dataManager.js';

class WhatsAppSender {
    constructor() {
        this.baseUrl = 'https://wa.me/';
        this.confirmationDelay = 3000; // 3 segundos
        this.massMessageDelay = 8000; // 8 segundos entre mensagens em massa
    }

    // Formata telefone para WhatsApp (conforme especificação)
    formatPhone(phone) {
        if (!phone || phone === '0' || phone === '') return null;
        
        let cleaned = phone.replace(/\D/g, '');
        
        // Remove zero inicial se existir
        if (cleaned.startsWith('0')) {
            cleaned = cleaned.substring(1);
        }
        
        // Casos especiais para Bahia e Sergipe (DDD 75, 71, 79)
        const validDDDs = ['75', '71', '79'];
        
        if (cleaned.length === 11 && validDDDs.some(ddd => cleaned.startsWith(ddd))) {
            // Número completo com DDD, adiciona código do país
            cleaned = '55' + cleaned;
        } else if (cleaned.length === 10 && validDDDs.some(ddd => cleaned.startsWith(ddd))) {
            // Número com DDD mas sem o 9 do celular, adiciona
            cleaned = '55' + cleaned.substring(0, 2) + '9' + cleaned.substring(2);
        } else if (cleaned.length === 9 && !cleaned.startsWith('55')) {
            // Só o número do celular, adiciona DDD 75 e código do país
            cleaned = '5575' + cleaned;
        } else if (cleaned.length === 8 && !cleaned.startsWith('55')) {
            // Número fixo, adiciona DDD 75, 9 e código do país
            cleaned = '55759' + cleaned;
        } else if (!cleaned.startsWith('55')) {
            // Qualquer outro caso, tenta adicionar 55 + 75
            if (cleaned.length >= 8) {
                cleaned = '5575' + cleaned;
            }
        }
        
        // Verifica se tem tamanho válido para WhatsApp (12-15 dígitos)
        if (cleaned.length >= 12 && cleaned.length <= 15 && cleaned.startsWith('55')) {
            return cleaned;
        }
        
        console.warn('⚠️ Telefone inválido:', phone, '-> formatado:', cleaned);
        return null;
    }

    // Gera mensagem personalizada
    generateMessage(company) {
        const template = document.getElementById('messageTemplate')?.value || '';
        
        if (!template) {
            console.error('❌ Template de mensagem não encontrado');
            return '';
        }
        
        return template
            .replace(/{EMPRESA}/g, company.name)
            .replace(/{RESPONSAVEL}/g, company.owner || 'o responsável');
    }

    // Envia mensagem individual
    async sendMessage(index) {
        const company = DataManager.getCompany(index);
        if (!company) {
            console.error('❌ Empresa não encontrada:', index);
            return false;
        }

        const phone = this.formatPhone(company.phone);
        
        if (!phone) {
            alert(
                `📱 TELEFONE INVÁLIDO\n\n` +
                `Empresa: ${company.name}\n` +
                `Telefone: ${company.phone}\n\n` +
                `Por favor, edite a empresa e corrija o telefone.`
            );
            return false;
        }

        const message = this.generateMessage(company);
        const whatsappUrl = `${this.baseUrl}${phone}?text=${encodeURIComponent(message)}`;
        
        console.log(`📱 Enviando para ${company.name}: ${phone}`);
        
        // Abre WhatsApp
        window.open(whatsappUrl, '_blank');

        // Confirma envio após delay
        setTimeout(() => {
            this.confirmSentMessage(index, company, phone);
        }, this.confirmationDelay);
        
        return true;
    }

    // Confirma se mensagem foi enviada
    confirmSentMessage(index, company, phone) {
        const wasSent = confirm(
            `📱 CONFIRMAÇÃO DE ENVIO\n\n` +
            `Empresa: ${company.name}\n` +
            `Telefone: ${phone}\n\n` +
            `✅ A mensagem foi enviada com sucesso no WhatsApp?\n\n` +
            `Clique "OK" se enviou ou "Cancelar" se não enviou.`
        );
        
        if (wasSent) {
            DataManager.updateCompany(index, { status: 'sent' });
            
            this.dispatchEvent('toast', {
                message: `✅ Mensagem confirmada: ${company.name}`,
                type: 'success'
            });
            
            this.dispatchEvent('updateUI');
            console.log(`✅ Confirmado envio para: ${company.name}`);
        } else {
            console.log(`❌ Envio cancelado para: ${company.name}`);
        }
    }

    // Marca empresa como respondeu
    markAsReplied(index) {
        const company = DataManager.getCompany(index);
        if (!company) return false;

        DataManager.updateCompany(index, { status: 'replied' });
        
        this.dispatchEvent('toast', {
            message: `📞 ${company.name} marcada como respondeu`,
            type: 'info'
        });
        
        this.dispatchEvent('updateUI');
        console.log(`📞 Marcada como respondeu: ${company.name}`);
        return true;
    }

    // Envia mensagens em massa
    async sendAllMessages() {
        const pendingCompanies = DataManager.companies.filter(c => 
            c.status === 'pending' && this.formatPhone(c.phone)
        );
        
        if (pendingCompanies.length === 0) {
            alert('📱 Não há empresas pendentes com telefones válidos.');
            return;
        }

        const confirmMessage = 
            `🚀 ENVIO EM MASSA\n\n` +
            `📊 ${pendingCompanies.length} empresas serão contactadas\n` +
            `⏰ Intervalo de ${this.massMessageDelay / 1000}s entre mensagens\n` +
            `⏱️ Tempo total estimado: ${Math.ceil(pendingCompanies.length * this.massMessageDelay / 60000)} minutos\n\n` +
            `O WhatsApp vai abrir várias abas em sequência.\n` +
            `Envie as mensagens e confirme quando solicitado.\n\n` +
            `Continuar?`;

        if (!confirm(confirmMessage)) return;

        let processedCount = 0;
        const startTime = Date.now();
        
        this.dispatchEvent('toast', {
            message: `🚀 Iniciando envio em massa para ${pendingCompanies.length} empresas`,
            type: 'info'
        });

        for (let i = 0; i < pendingCompanies.length; i++) {
            const company = pendingCompanies[i];
            const realIndex = DataManager.companies.findIndex(c => c.serial === company.serial);
            
            await this.sendMassMessage(company, realIndex, i + 1, pendingCompanies.length);
            
            // Aguarda confirmação
            const confirmed = await this.waitForConfirmation(company, 4000);
            
            if (confirmed) {
                DataManager.updateCompany(realIndex, { status: 'sent' });
                processedCount++;
            }
            
            this.dispatchEvent('updateUI');
            
            // Delay entre mensagens (exceto na última)
            if (i < pendingCompanies.length - 1) {
                await this.delay(this.massMessageDelay - 4000);
            }
        }
        
        // Resultado final
        const endTime = Date.now();
        const duration = Math.round((endTime - startTime) / 1000);
        
        DataManager.autoSaveJSON();
        
        alert(
            `🎉 ENVIO EM MASSA CONCLUÍDO!\n\n` +
            `✅ ${processedCount} mensagens confirmadas\n` +
            `❌ ${pendingCompanies.length - processedCount} não enviadas\n` +
            `⏱️ Tempo total: ${Math.floor(duration / 60)}:${(duration % 60).toString().padStart(2, '0')}\n\n` +
            `Dados salvos automaticamente.`
        );
        
        console.log(`📊 Envio em massa concluído: ${processedCount}/${pendingCompanies.length}`);
    }

    // Envia mensagem individual no processo em massa
    async sendMassMessage(company, realIndex, current, total) {
        const phone = this.formatPhone(company.phone);
        const message = this.generateMessage(company);
        const whatsappUrl = `${this.baseUrl}${phone}?text=${encodeURIComponent(message)}`;
        
        console.log(`📱 Envio em massa ${current}/${total}: ${company.name}`);
        window.open(whatsappUrl, '_blank');
        
        this.dispatchEvent('toast', {
            message: `📱 Enviando ${current}/${total}: ${company.name}`,
            type: 'info'
        });
    }

    // Aguarda confirmação do usuário
    async waitForConfirmation(company, delay) {
        await this.delay(delay);
        
        return confirm(
            `📱 CONFIRMAÇÃO DE ENVIO\n\n` +
            `Empresa: ${company.name}\n\n` +
            `✅ Esta mensagem foi enviada no WhatsApp?\n\n` +
            `Clique "OK" se enviou ou "Cancelar" se não enviou.`
        );
    }

    // Testa formatação do telefone
    testPhone(index) {
        const company = DataManager.getCompany(index);
        if (!company) return;

        const originalPhone = company.phone;
        const formattedPhone = this.formatPhone(originalPhone);
        
        let message = `📱 TESTE DE FORMATAÇÃO\n\n`;
        message += `Empresa: ${company.name}\n`;
        message += `Telefone original: ${originalPhone}\n`;
        message += `Telefone formatado: ${formattedPhone || 'INVÁLIDO'}\n\n`;
        
        if (formattedPhone) {
            message += `✅ Telefone válido para WhatsApp\n`;
            message += `🔗 URL: ${this.baseUrl}${formattedPhone}\n\n`;
            message += `Deseja testar abrindo o WhatsApp?`;
            
            if (confirm(message)) {
                window.open(`${this.baseUrl}${formattedPhone}`, '_blank');
            }
        } else {
            message += `❌ Telefone inválido\n\n`;
            message += `Formatos aceitos:\n`;
            message += `• (75) 99999-9999\n`;
            message += `• 75999999999\n`;
            message += `• 999999999`;
            
            alert(message);
        }
    }

    // Obtém empresas com telefones válidos
    getValidPhoneCompanies() {
        return DataManager.companies.filter(company => 
            this.formatPhone(company.phone) !== null
        );
    }

    // Obtém empresas com telefones inválidos
    getInvalidPhoneCompanies() {
        return DataManager.companies.filter(company => 
            this.formatPhone(company.phone) === null
        );
    }

    // Valida telefone
    isValidPhone(phone) {
        return this.formatPhone(phone) !== null;
    }

    // Utilitários
    delay(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    // Sistema de eventos
    dispatchEvent(eventType, data = {}) {
        const event = new CustomEvent(`whatsappSender:${eventType}`, {
            detail: data
        });
        document.dispatchEvent(event);
    }

    // Obtém estatísticas de telefones
    getPhoneStats() {
        const total = DataManager.companies.length;
        const valid = this.getValidPhoneCompanies().length;
        const invalid = this.getInvalidPhoneCompanies().length;
        
        return {
            total,
            valid,
            invalid,
            validPercentage: Math.round((valid / total) * 100)
        };
    }

    // Gera relatório de telefones
    generatePhoneReport() {
        const stats = this.getPhoneStats();
        const invalidCompanies = this.getInvalidPhoneCompanies();
        
        let report = `📊 RELATÓRIO DE TELEFONES\n\n`;
        report += `Total de empresas: ${stats.total}\n`;
        report += `Telefones válidos: ${stats.valid} (${stats.validPercentage}%)\n`;
        report += `Telefones inválidos: ${stats.invalid}\n\n`;
        
        if (invalidCompanies.length > 0) {
            report += `❌ EMPRESAS COM TELEFONES INVÁLIDOS:\n\n`;
            invalidCompanies.forEach((company, index) => {
                report += `${index + 1}. ${company.name}\n`;
                report += `   Telefone: ${company.phone}\n`;
                report += `   CNPJ: ${company.cnpj}\n\n`;
            });
        }
        
        return report;
    }

    // Configura parâmetros
    configure(options = {}) {
        if (options.confirmationDelay) {
            this.confirmationDelay = Math.max(1000, options.confirmationDelay);
        }
        
        if (options.massMessageDelay) {
            this.massMessageDelay = Math.max(5000, options.massMessageDelay);
        }
        
        console.log('⚙️ WhatsApp configurado:', {
            confirmationDelay: this.confirmationDelay,
            massMessageDelay: this.massMessageDelay
        });
    }
}

// Exporta instância única
const whatsappSender = new WhatsAppSender();
export default whatsappSender;