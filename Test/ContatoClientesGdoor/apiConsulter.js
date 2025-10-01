// ========================================
// MÓDULO DE CONSULTA DE API CNPJ
// ========================================

import DataManager from './dataManager.js';

class APIConsulter {
    constructor() {
        this.apiUrl = 'https://open.cnpja.com/office/';
        this.requestDelay = 1000; // 1 segundo entre requisições
        this.maxRetries = 3;
    }

    // Consulta uma empresa específica
    async consultCompany(index) {
        const company = DataManager.getCompany(index);
        if (!company) {
            console.error('❌ Empresa não encontrada no índice:', index);
            return false;
        }

        const cleanCNPJ = this.cleanCNPJ(company.cnpj);
        
        try {
            this.dispatchEvent('toast', {
                message: `Consultando API: ${company.name}`,
                type: 'info'
            });
            
            const apiData = await this.fetchCNPJData(cleanCNPJ);
            const processedData = this.processAPIData(apiData, company);
            
            // Atualiza a empresa
            DataManager.updateCompany(index, processedData);
            
            this.dispatchEvent('toast', {
                message: `✅ Dados atualizados: ${processedData.name}`,
                type: 'success'
            });
            
            console.log(`✅ API: ${company.name} atualizada`);
            return true;
            
        } catch (error) {
            console.error('❌ Erro ao consultar API:', error);
            this.dispatchEvent('toast', {
                message: `❌ Erro: ${company.name} - ${error.message}`,
                type: 'warning'
            });
            return false;
        }
    }

    // Faz requisição para a API
    async fetchCNPJData(cnpj, retryCount = 0) {
        try {
            const response = await fetch(`${this.apiUrl}${cnpj}`);
            
            if (!response.ok) {
                if (response.status === 429 && retryCount < this.maxRetries) {
                    // Rate limit - aguarda e tenta novamente
                    await this.delay(this.requestDelay * (retryCount + 2));
                    return this.fetchCNPJData(cnpj, retryCount + 1);
                }
                throw new Error(`API retornou erro ${response.status}`);
            }
            
            const data = await response.json();
            this.validateAPIResponse(data);
            return data;
            
        } catch (error) {
            if (retryCount < this.maxRetries) {
                console.warn(`⚠️ Tentativa ${retryCount + 1} falhou, tentando novamente...`);
                await this.delay(this.requestDelay * (retryCount + 1));
                return this.fetchCNPJData(cnpj, retryCount + 1);
            }
            throw error;
        }
    }

    // Processa dados da API conforme especificação
    processAPIData(apiData, currentCompany) {
        // Nome da empresa
        const name = apiData.company?.name || currentCompany.name;
        
        // Processa emails
        const emails = apiData.emails?.map(email => email.address).join(', ') || currentCompany.email || '';
        
        // Processa telefones (atualiza apenas se atual for inválido)
        let phone = currentCompany.phone;
        if (apiData.phones && apiData.phones.length > 0) {
            const apiPhone = apiData.phones[0];
            if (apiPhone.area && apiPhone.number) {
                const newPhone = `(${apiPhone.area}) ${apiPhone.number}`;
                // Só atualiza se o telefone atual for inválido
                if (!this.isValidPhone(currentCompany.phone)) {
                    phone = newPhone;
                }
            }
        }
        
        // Processa sócios (apenas primeiro nome, separado por barra)
        const members = apiData.company?.members || [];
        const owners = this.extractOwners(members);
        
        // Monta observações com informações da API
        const notes = this.buildNotes(apiData, currentCompany.notes);
        
        return {
            name: name,
            email: emails,
            phone: phone,
            owner: owners,
            notes: notes
        };
    }

    // Extrai responsáveis/sócios conforme especificação
    extractOwners(members) {
        return members
            .filter(member => {
                const role = member.role?.text?.toLowerCase() || '';
                return role.includes('sóci') || 
                       role.includes('diretor') || 
                       role.includes('admin') ||
                       role.includes('gerente') ||
                       role.includes('presidente');
            })
            .map(member => {
                const fullName = member.person?.name || '';
                return fullName.split(' ')[0]; // Apenas primeiro nome
            })
            .filter(name => name && name.length > 1) // Remove nomes muito curtos
            .slice(0, 5) // Máximo 5 sócios
            .join(' / '); // Separado por barra conforme solicitado
    }

    // Constrói observações com dados da API
    buildNotes(apiData, currentNotes = '') {
        const status = apiData.status?.text || '';
        const activity = apiData.mainActivity?.text || '';
        const address = `${apiData.address?.city || ''}, ${apiData.address?.state || ''}`;
        const updateDate = new Date().toLocaleDateString('pt-BR');
        
        let notes = `Status: ${status}\n`;
        notes += `Atividade: ${activity}\n`;
        notes += `Endereço: ${address}\n`;
        notes += `Atualizado via API em ${updateDate}`;
        
        // Preserva observações anteriores se existirem
        if (currentNotes && !currentNotes.includes('Atualizado via API')) {
            notes = `${currentNotes}\n\n--- DADOS DA API ---\n${notes}`;
        }
        
        return notes;
    }

    // Atualiza todas as empresas em massa
    async updateAllCompanies() {
        const pendingCompanies = DataManager.companies.filter(c => 
            !c.owner || !c.email || c.owner.trim() === '' || c.email.trim() === ''
        );
        
        if (pendingCompanies.length === 0) {
            alert('✅ Todas as empresas já possuem dados completos!');
            return;
        }
        
        const confirmUpdate = confirm(
            `🔄 ATUALIZAÇÃO EM MASSA VIA API\n\n` +
            `📊 ${pendingCompanies.length} empresas serão consultadas\n` +
            `⏰ Processo pode levar alguns minutos\n` +
            `🌐 API: open.cnpja.com\n` +
            `⚡ Delay de ${this.requestDelay}ms entre requisições\n\n` +
            `Continuar?`
        );
        
        if (!confirmUpdate) return;
        
        let successCount = 0;
        let errorCount = 0;
        const startTime = Date.now();
        
        this.dispatchEvent('toast', {
            message: `🚀 Iniciando atualização de ${pendingCompanies.length} empresas...`,
            type: 'info'
        });
        
        for (let i = 0; i < pendingCompanies.length; i++) {
            const company = pendingCompanies[i];
            const realIndex = DataManager.companies.findIndex(c => c.serial === company.serial);
            
            const success = await this.consultCompany(realIndex);
            
            if (success) {
                successCount++;
            } else {
                errorCount++;
            }
            
            // Delay entre requisições
            if (i < pendingCompanies.length - 1) {
                await this.delay(this.requestDelay);
            }
            
            // Atualiza progresso a cada 5 empresas
            if ((i + 1) % 5 === 0 || i === pendingCompanies.length - 1) {
                const progress = Math.round(((i + 1) / pendingCompanies.length) * 100);
                this.dispatchEvent('toast', {
                    message: `📊 Progresso: ${i + 1}/${pendingCompanies.length} (${progress}%)`,
                    type: 'info'
                });
                
                // Dispara evento para atualizar interface
                this.dispatchEvent('updateUI');
            }
        }
        
        // Salva dados finais
        DataManager.autoSaveJSON();
        
        const endTime = Date.now();
        const duration = Math.round((endTime - startTime) / 1000);
        
        // Resultado final
        alert(
            `🎉 ATUALIZAÇÃO CONCLUÍDA!\n\n` +
            `✅ ${successCount} empresas atualizadas\n` +
            `❌ ${errorCount} empresas com erro\n` +
            `⏱️ Tempo total: ${duration}s\n\n` +
            `Dados salvos automaticamente.`
        );
        
        console.log(`📊 Atualização concluída: ${successCount}/${pendingCompanies.length} empresas`);
    }

    // Consulta individual com feedback visual
    async consultCompanyWithFeedback(index) {
        const button = document.querySelector(`[onclick*="consultAPI(${index})"]`);
        const originalHTML = button?.innerHTML;
        
        if (button) {
            button.innerHTML = '<i class="fas fa-spinner fa-spin"></i>';
            button.disabled = true;
        }
        
        try {
            const result = await this.consultCompany(index);
            return result;
        } finally {
            if (button) {
                button.innerHTML = originalHTML;
                button.disabled = false;
            }
        }
    }

    // Valida resposta da API
    validateAPIResponse(data) {
        if (!data || typeof data !== 'object') {
            throw new Error('Resposta da API inválida');
        }
        
        if (!data.company && !data.taxId) {
            throw new Error('Dados da empresa não encontrados');
        }
        
        return true;
    }

    // Utilitários
    cleanCNPJ(cnpj) {
        return cnpj.replace(/\D/g, '');
    }

    isValidPhone(phone) {
        if (!phone || phone === '0' || phone === '') return false;
        const cleaned = phone.replace(/\D/g, '');
        return cleaned.length >= 10;
    }

    delay(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    // Sistema de eventos
    dispatchEvent(eventType, data = {}) {
        const event = new CustomEvent(`apiConsulter:${eventType}`, {
            detail: data
        });
        document.dispatchEvent(event);
    }

    // Obtém estatísticas da API
    getAPIStats() {
        return {
            baseUrl: this.apiUrl,
            requestDelay: this.requestDelay,
            maxRetries: this.maxRetries,
            status: 'Operacional'
        };
    }

    // Configura parâmetros da API
    configure(options = {}) {
        if (options.requestDelay) {
            this.requestDelay = Math.max(500, options.requestDelay);
        }
        
        if (options.maxRetries) {
            this.maxRetries = Math.max(1, Math.min(5, options.maxRetries));
        }
        
        console.log('⚙️ API configurada:', {
            requestDelay: this.requestDelay,
            maxRetries: this.maxRetries
        });
    }

    // Testa conexão com a API
    async testConnection() {
        try {
            const testCNPJ = '11222333000181'; // CNPJ de teste
            await this.fetchCNPJData(testCNPJ);
            return true;
        } catch (error) {
            console.error('❌ Teste de conexão API falhou:', error);
            return false;
        }
    }

    // Gera relatório de empresas sem dados
    generateMissingDataReport() {
        const missingData = DataManager.companies.filter(c => 
            !c.owner || !c.email || c.owner.trim() === '' || c.email.trim() === ''
        );
        
        let report = `📊 RELATÓRIO DE DADOS FALTANTES\n\n`;
        report += `Total de empresas: ${DataManager.companies.length}\n`;
        report += `Empresas com dados incompletos: ${missingData.length}\n\n`;
        
        if (missingData.length > 0) {
            report += `❌ EMPRESAS COM DADOS FALTANTES:\n\n`;
            missingData.forEach((company, index) => {
                report += `${index + 1}. ${company.name}\n`;
                report += `   CNPJ: ${company.cnpj}\n`;
                report += `   Responsável: ${company.owner || 'Não informado'}\n`;
                report += `   Email: ${company.email || 'Não informado'}\n\n`;
            });
        }
        
        return report;
    }
}

// Exporta instância única
const apiConsulter = new APIConsulter();
export default apiConsulter;